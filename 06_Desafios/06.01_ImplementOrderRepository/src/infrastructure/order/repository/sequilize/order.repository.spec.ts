import { Sequelize } from "sequelize-typescript";
import Order from "../../../../domain/checkout/entity/order";
import OrderItem from "../../../../domain/checkout/entity/order_item";
import Customer from "../../../../domain/customer/entity/customer";
import Address from "../../../../domain/customer/value-object/address";
import Product from "../../../../domain/product/entity/product";
import CustomerModel from "../../../customer/repository/sequelize/customer.model";
import CustomerRepository from "../../../customer/repository/sequelize/customer.repository";
import ProductModel from "../../../product/repository/sequelize/product.model";
import ProductRepository from "../../../product/repository/sequelize/product.repository";
import OrderItemModel from "./order-item.model";
import OrderModel from "./order.model";
import OrderRepository from "./order.repository";

describe("Order repository test", () => {
  let sequelize: Sequelize;

  beforeEach(async () => {
    sequelize = new Sequelize({
      dialect: "sqlite",
      storage: ":memory:",
      logging: false,
      sync: { force: true },
    });

    await sequelize.addModels([
      CustomerModel,
      OrderModel,
      OrderItemModel,
      ProductModel,
    ]);
    await sequelize.sync();
  });

  afterEach(async () => {
    await sequelize.close();
  });

  it("should create a new order", async () => {

    const customer = new Customer("1", "Customer 1");
    customer.changeAddress(new Address("Street 1", 1, "Zipcode 1", "City 1"));
    await new CustomerRepository().create(customer);

    const product = new Product("123", "Product 1", 10);
    await new ProductRepository().create(product);

    const ordemItem = new OrderItem("1", product.name, product.price, product.id, 2);
    const order = new Order("1", customer.id, [ordemItem]);
    await new OrderRepository().create(order);

    const orderModel = await OrderModel.findOne({
      where: { id: order.id },
      include: ["items"],
    });

    expect(orderModel.toJSON()).toStrictEqual({
      id: order.id,
      customer_id: customer.id,
      total: order.total(),
      items: [
        {
          id: ordemItem.id,
          name: ordemItem.name,
          unitaryPrice: ordemItem.unitaryPrice,
          quantity: ordemItem.quantity,
          order_id: order.id,
          product_id: product.id
        },
      ],
    });
  });

  it("should update exists order", async () => {

    const customer = new Customer("c1", "New Customer");
    customer.changeAddress(new Address("Praça da Sé", 110, "01001-000", "São Paulo"));
    await new CustomerRepository().create(customer);

    const productCoke = new Product("p1", "Coke", 6.70);
    await new ProductRepository().create(productCoke);

    const orderItemCoke = new OrderItem("1", productCoke.name, productCoke.price, productCoke.id, 2);

    const order = new Order("o1", customer.id, [orderItemCoke]);
    const orderRepository = new OrderRepository();
    await orderRepository.create(order);

    let orderDb = await orderRepository.find(order.id);

    expect(orderDb.id).toBe("o1");
    expect(orderDb.customerId).toBe("c1");
    expect(orderDb.total()).toBe(13.4);
    expect(orderDb.items.length).toBe(1);

    const changedCustomer = new Customer("c2", "Changed Customer");
    changedCustomer.changeAddress(new Address("Praça da Sé", 110, "01001-000", "São Paulo"));
    await new CustomerRepository().create(changedCustomer);

    const changedOrder = new Order(orderDb.id, changedCustomer.id, [orderItemCoke]);
    await orderRepository.update(changedOrder);
    orderDb = await orderRepository.find(order.id);

    expect(orderDb.id).toBe("o1");
    expect(orderDb.customerId).toBe("c2");
    expect(orderDb.total()).toBe(13.4);
    expect(orderDb.items.length).toBe(1);
  });

  it("should find order", async () => {

    const customer = new Customer("c1", "New Customer");
    customer.changeAddress(new Address("Praça da Sé", 110, "01001-000", "São Paulo"));
    await new CustomerRepository().create(customer);

    const productCoke = new Product("p1", "Coke", 6.7);
    await new ProductRepository().create(productCoke);

    const orderItemCoke = new OrderItem("1", productCoke.name, productCoke.price, productCoke.id, 2);
    const order = new Order("o1", customer.id, [orderItemCoke]);
    const orderRepository = new OrderRepository();
    await orderRepository.create(order);

    const orderDb = await orderRepository.find(order.id);

    expect(orderDb.id).toBe("o1");
    expect(orderDb.items.length).toBe(1);
  });

  it("should find all orders", async () => {

    const customer = new Customer("c1", "New Customer");
    customer.changeAddress(new Address("Praça da Sé", 110, "01001-000", "São Paulo"));
    await new CustomerRepository().create(customer);

    const productCoke = new Product("p1", "Coke", 6.7);
    await new ProductRepository().create(productCoke);

    const orderRepository = new OrderRepository();

    const orderItemCoke = new OrderItem("1", productCoke.name, productCoke.price, productCoke.id, 1);
    const order1 = new Order("o1", customer.id, [orderItemCoke]);
    await orderRepository.create(order1);

    const orderItemCoke2 = new OrderItem("2", productCoke.name, productCoke.price, productCoke.id, 1);
    const order2 = new Order("o2", customer.id, [orderItemCoke2]);
    await orderRepository.create(order2);

    const ordersDb = await orderRepository.findAll();
    const ordersLocal = [order1, order2];

    expect(ordersLocal).toEqual(ordersDb);
  });
});