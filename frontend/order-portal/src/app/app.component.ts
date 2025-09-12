import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OrderComponent } from "./order/order.component";

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [OrderComponent]
})
export class AppComponent {
  title = 'order-portal';
}
