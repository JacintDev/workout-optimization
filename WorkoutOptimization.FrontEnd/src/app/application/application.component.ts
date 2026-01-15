import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Promotion } from '../../models/Promotion';
import { environment } from '../../environment/environment';

@Component({
  selector: 'app-application',
  standalone: false,
  templateUrl: './application.component.html',
  styleUrl: './application.component.sass',
})
export class ApplicationComponent implements OnInit {
  http: HttpClient;
  Promotions: Array<Promotion>;

  constructor(http: HttpClient) {
    this.http = http;
    this.Promotions = new Array<Promotion>();
  }
  ngOnInit(): void {
    this.http
      .get<Array<Promotion>>(`${environment.apiUrl}api/Promotion`)
      .subscribe((success) => {
        success.map((x: Promotion) => {
          let promo = new Promotion();
          promo.promotionId = x.promotionId;
          promo.name = x.name;
          promo.description = x.description;
          promo.image = x.image;
          this.Promotions.push(promo);
        }),
          (error: any) => {
            console.log(error.message);
          };
      });
    console.log(this.Promotions);
  }
}
