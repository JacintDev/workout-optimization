import { Component, ElementRef, AfterViewInit, ViewChild } from '@angular/core';
import * as THREE from 'three';
// @ts-ignore
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

@Component({
  selector: 'app-biceps-curl-animate',
  standalone: false,
  templateUrl: './biceps-curl-animate.component.html',
  styleUrls: ['./biceps-curl-animate.component.sass'],
})
export class BicepsCurlAnimateComponent implements AfterViewInit {
  @ViewChild('rendererContainer', { static: true })
  rendererContainer!: ElementRef;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private mixer!: THREE.AnimationMixer;
  private clock = new THREE.Clock();
  private controls!: OrbitControls;

  ngAfterViewInit(): void {
    this.initThree();
    this.loadModel();
    this.animate();
  }

  private initThree() {
    // Scene
    this.scene = new THREE.Scene();
    this.scene.background = null;

    // Camera
    const width = this.rendererContainer.nativeElement.clientWidth;
    const height = this.rendererContainer.nativeElement.clientHeight;
    this.camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
    this.camera.position.set(0, 2, 3.5);

    // Renderer
    this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
    this.renderer.setClearColor(0x000000, 0);
    this.renderer.setSize(width, height);
    this.rendererContainer.nativeElement.appendChild(this.renderer.domElement);

    // Lights
    const hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444, 1);
    hemiLight.position.set(0, 20, 0);
    this.scene.add(hemiLight);

    const dirLight = new THREE.DirectionalLight(0xffffff, 0.8);
    dirLight.position.set(5, 10, 7.5);
    this.scene.add(dirLight);

    // OrbitControls (egérrel forgatás)
    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.target.set(0, 1, 0);
    this.controls.update();
  }

  // private loadModel() {
  //   const loader = new GLTFLoader();
  //   loader.load('BicepsCurl.glb', (gltf) => {
  //     const model = gltf.scene;
  //     model.scale.set(2, 2, 2);
  //     this.scene.add(model);

  //     // Animáció
  //     if (gltf.animations && gltf.animations.length > 0) {
  //       this.mixer = new THREE.AnimationMixer(model);

  //       // Az első animáció lejátszása
  //       const action = this.mixer.clipAction(gltf.animations[0]);
  //       action.play();

  //       // Ha konkrét animációt akarsz név alapján:
  //       // const clip = THREE.AnimationClip.findByName(gltf.animations, 'BicepsCurl');
  //       // if (clip) this.mixer.clipAction(clip).play();
  //     }
  //   });
  // }
  private loadModel() {
    const loader = new GLTFLoader();
    loader.load('BicepsCurl.glb', (gltf) => {
      const model = gltf.scene;

      // Méretezés
      model.scale.set(2, 2, 2);

      // Bounding box
      const box = new THREE.Box3().setFromObject(model);
      const size = box.getSize(new THREE.Vector3());
      const center = box.getCenter(new THREE.Vector3());

      // Modell lefelé tolása, hogy az alja y=0-ra kerüljön
      model.position.y -= box.min.y;

      // Kamera a modell közepére néz, de kicsit lejjebb toljuk, hogy has köré essen
      const hasY = box.min.y + size.y * 0.5; // 40%-kal a modell alja felett, has környék
      this.controls.target.set(center.x, hasY, center.z);
      this.controls.update();

      this.scene.add(model);

      // Animáció
      if (gltf.animations && gltf.animations.length > 0) {
        this.mixer = new THREE.AnimationMixer(model);
        const action = this.mixer.clipAction(gltf.animations[0]);
        action.play();
      }
    });
  }
  private animate = () => {
    requestAnimationFrame(this.animate);

    const delta = this.clock.getDelta();
    if (this.mixer) this.mixer.update(delta);

    this.renderer.render(this.scene, this.camera);
  };
}
