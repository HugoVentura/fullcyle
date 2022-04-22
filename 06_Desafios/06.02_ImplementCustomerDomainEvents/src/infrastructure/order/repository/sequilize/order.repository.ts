import Order from "../../../../domain/checkout/entity/order";
import OrderItem from "../../../../domain/checkout/entity/order_item";
import OrderRepositoryInterface from "../../../../domain/checkout/repository/order-repository.interface";
import OrderItemModel from "./order-item.model";
import OrderModel from "./order.model";

export default class OrderRepository implements OrderRepositoryInterface {

  async create(entity: Order): Promise<void> {
    await OrderModel.create(
      {
        id: entity.id,
        customer_id: entity.customerId,
        total: entity.total(),
        items: entity.items.map((item) => ({
          id: item.id,
          name: item.name,
          unitaryPrice: item.unitaryPrice,
          product_id: item.productId,
          quantity: item.quantity,
        })),
      },
       {
         include: [{ model: OrderItemModel }],
       }
    );
  }

  async update(entity: Order): Promise<void> {
    await OrderModel.update(
      {
        id: entity.id,
        customer_id: entity.customerId,
        total: entity.total(),
        items: entity.items.map((item) => ({
          id: item.id,
          name: item.name,
          price: item.unitaryPrice,
          product_id: item.productId,
          quantity: item.quantity,
        })),
      },
      {
        where: {
          id: entity.id,
        },
      }
    );
  }

  async find(id: string): Promise<Order> {
    let orderModel;
    try {
      orderModel = await OrderModel.findOne({
        where: {
          id,
        },
        include: [{ model: OrderItemModel }],
        rejectOnEmpty: true,
      });
    } catch (error) {
      throw new Error("Order not found");
    }

    const orderItems = orderModel.items.map((item) => {
      return new OrderItem(item.id, item.name, item.unitaryPrice, item.product_id, item.quantity);
    });

    return new Order(orderModel.id, orderModel.customer_id, orderItems);
  }

  async findAll(): Promise<Order[]> {
    let orderModels;
    try {
      orderModels = await OrderModel.findAll({
        include: [{ model: OrderItemModel }],
      });
    } catch (error) {
      throw new Error("Orders not found");
    }

    const orders = orderModels.map((orderModel) => {
      const orderItems = orderModel.items.map((item) => {
        return new OrderItem(item.id, item.name, item.unitaryPrice, item.product_id, item.quantity);
      });

      return new Order(orderModel.id, orderModel.customer_id, orderItems);
    });

    return orders;
  }
}
