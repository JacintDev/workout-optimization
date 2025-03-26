import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatTabGroup } from '@angular/material/tabs';

@Component({
  selector: 'app-landing-page',
  standalone: false,
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.sass',
})
export class LandingPageComponent implements OnInit, OnDestroy {
  @ViewChild(MatTabGroup) tabGroup!: MatTabGroup;
  intervalId: any;
  currentIndex = 0;

  ngOnInit(): void {
    this.startAutoSwitch();
    this.deleteTab();
  }

  startAutoSwitch(): void {
    this.intervalId = setInterval(() => {
      this.currentIndex = (this.currentIndex + 1) % this.tabGroup._tabs.length;
      this.deleteTab();
    }, 3000); // 3 másodpercenként vált
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  deleteTab(): void {
    document.querySelector('.mat-mdc-tab-header')?.remove();
    // document.querySelector('.mat-mdc-tab-header').style.display = 'none';
  }
}
