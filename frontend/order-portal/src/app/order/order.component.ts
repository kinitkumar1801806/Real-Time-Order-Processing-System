import { Component } from '@angular/core';
import { OrderService } from './order.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  standalone : true,
  imports: [FormsModule]
})
export class OrderComponent {
  order = { customer: '', amount: 0 };
  message = '';

  constructor(private orderService: OrderService) {}

  placeOrder() {
    this.orderService.placeOrder(this.order).subscribe(res => {
      this.message = res.message;
    });
  }
}
