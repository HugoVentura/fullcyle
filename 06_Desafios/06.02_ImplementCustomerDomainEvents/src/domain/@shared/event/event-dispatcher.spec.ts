import CustomerChangeAddressEvent from "../../customer/event/customer-change-address.event";
import CustomerCreatedEvent from "../../customer/event/customer-created.event";
import ChangeAddressLogHandler from "../../customer/event/handler/change-address-log.handler";
import CreatedUserLog1Handler from "../../customer/event/handler/created-user-log-1.handler";
import CreatedUserLog2Handler from "../../customer/event/handler/created-user-log-2.handler";
import SendEmailWhenProductIsCreatedHandler from "../../product/event/handler/send-email-when-product-is-created.handler";
import ProductCreatedEvent from "../../product/event/product-created.event";
import EventDispatcher from "./event-dispatcher";

describe("Domain events tests", () => {
  it("should register an event handler", () => {
    const eventDispatcher = new EventDispatcher();
    const eventHandler = new SendEmailWhenProductIsCreatedHandler();

    eventDispatcher.register("ProductCreatedEvent", eventHandler);

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"]
    ).toBeDefined();
    expect(eventDispatcher.getEventHandlers["ProductCreatedEvent"].length).toBe(
      1
    );
    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"][0]
    ).toMatchObject(eventHandler);
  });

  it("should unregister an event handler", () => {
    const eventDispatcher = new EventDispatcher();
    const eventHandler = new SendEmailWhenProductIsCreatedHandler();

    eventDispatcher.register("ProductCreatedEvent", eventHandler);

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"][0]
    ).toMatchObject(eventHandler);

    eventDispatcher.unregister("ProductCreatedEvent", eventHandler);

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"]
    ).toBeDefined();
    expect(eventDispatcher.getEventHandlers["ProductCreatedEvent"].length).toBe(
      0
    );
  });

  it("should unregister all event handlers", () => {
    const eventDispatcher = new EventDispatcher();
    const eventHandler = new SendEmailWhenProductIsCreatedHandler();

    eventDispatcher.register("ProductCreatedEvent", eventHandler);

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"][0]
    ).toMatchObject(eventHandler);

    eventDispatcher.unregisterAll();

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"]
    ).toBeUndefined();
  });

  it("should notify all event handlers", () => {
    const eventDispatcher = new EventDispatcher();
    const eventHandler = new SendEmailWhenProductIsCreatedHandler();
    const spyEventHandler = jest.spyOn(eventHandler, "handle");

    eventDispatcher.register("ProductCreatedEvent", eventHandler);

    expect(
      eventDispatcher.getEventHandlers["ProductCreatedEvent"][0]
    ).toMatchObject(eventHandler);

    const productCreatedEvent = new ProductCreatedEvent({
      name: "Product 1",
      description: "Product 1 description",
      price: 10.0,
    });

    // Quando o notify for executado o SendEmailWhenProductIsCreatedHandler.handle() deve ser chamado
    eventDispatcher.notify(productCreatedEvent);

    expect(spyEventHandler).toHaveBeenCalled();
  });

  it("should notify when a new customer is created", () =>{
    const eventDispatcher = new EventDispatcher();
    const createdLog1Handler = new CreatedUserLog1Handler();
    const createdLog2Handler = new CreatedUserLog2Handler();
    const spyEvent1Handler = jest.spyOn(createdLog1Handler, "handle");
    const spyEvent2Handler = jest.spyOn(createdLog2Handler, "handle");

    eventDispatcher.register("CustomerCreatedEvent", createdLog1Handler);
    eventDispatcher.register("CustomerCreatedEvent", createdLog2Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][0]
    ).toMatchObject(createdLog1Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][1]
    ).toMatchObject(createdLog2Handler);

    eventDispatcher.notify(new CustomerCreatedEvent(null));

    expect(spyEvent1Handler).toHaveBeenCalled();
    expect(spyEvent2Handler).toHaveBeenCalled();
  })

  it("should notify when a customer address is changed", () =>{
    const eventDispatcher = new EventDispatcher();
    const changeAddressHandler = new ChangeAddressLogHandler();
    const spyEventHandler = jest.spyOn(changeAddressHandler, "handle");

    eventDispatcher.register("CustomerChangeAddressEvent", changeAddressHandler);

    expect(
      eventDispatcher.getEventHandlers["CustomerChangeAddressEvent"][0]
    ).toMatchObject(changeAddressHandler);

    const event = new CustomerChangeAddressEvent({
      id: "1",
      name: "Customer Name",
      address: `Street, 0, City - 00000-000`
    });

    eventDispatcher.notify(event);

    expect(spyEventHandler).toHaveBeenCalled();
  })

  it("should notify when a new customer is created but not when a address is changed", () =>{
    const eventDispatcher = new EventDispatcher();
    const createdLog1Handler = new CreatedUserLog1Handler();
    const createdLog2Handler = new CreatedUserLog2Handler();
    const changeAddressHandler = new ChangeAddressLogHandler();
    const spyEvent1Handler = jest.spyOn(createdLog1Handler, "handle");
    const spyEvent2Handler = jest.spyOn(createdLog2Handler, "handle");
    const spyEventChangeAddressHandler = jest.spyOn(changeAddressHandler, "handle");

    eventDispatcher.register("CustomerCreatedEvent", createdLog1Handler);
    eventDispatcher.register("CustomerCreatedEvent", createdLog2Handler);
    eventDispatcher.register("CustomerChangeAddressEvent", changeAddressHandler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][0]
    ).toMatchObject(createdLog1Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][1]
    ).toMatchObject(createdLog2Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerChangeAddressEvent"][0]
    ).toMatchObject(changeAddressHandler);

    eventDispatcher.notify(new CustomerCreatedEvent(null));

    expect(spyEvent1Handler).toHaveBeenCalled();
    expect(spyEvent2Handler).toHaveBeenCalled();
    expect(spyEventChangeAddressHandler).toBeCalledTimes(0);
  })

  it("should notify when a address is changed but not when a customer is created", () =>{
    const eventDispatcher = new EventDispatcher();
    const createdLog1Handler = new CreatedUserLog1Handler();
    const createdLog2Handler = new CreatedUserLog2Handler();
    const changeAddressHandler = new ChangeAddressLogHandler();
    const spyEvent1Handler = jest.spyOn(createdLog1Handler, "handle");
    const spyEvent2Handler = jest.spyOn(createdLog2Handler, "handle");
    const spyEventChangeAddressHandler = jest.spyOn(changeAddressHandler, "handle");

    eventDispatcher.register("CustomerCreatedEvent", createdLog1Handler);
    eventDispatcher.register("CustomerCreatedEvent", createdLog2Handler);
    eventDispatcher.register("CustomerChangeAddressEvent", changeAddressHandler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][0]
    ).toMatchObject(createdLog1Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][1]
    ).toMatchObject(createdLog2Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerChangeAddressEvent"][0]
    ).toMatchObject(changeAddressHandler);

    const event = new CustomerChangeAddressEvent({
      id: "1",
      name: "Customer Name",
      address: `Street, 0, City - 00000-000`
    });

    eventDispatcher.notify(event);

    expect(spyEventChangeAddressHandler).toHaveBeenCalled();
    expect(spyEvent1Handler).toBeCalledTimes(0);
    expect(spyEvent2Handler).toBeCalledTimes(0);
  })

  it("should notify when a new customer is created and when a address is changed", () =>{
    const eventDispatcher = new EventDispatcher();
    const createdLog1Handler = new CreatedUserLog1Handler();
    const createdLog2Handler = new CreatedUserLog2Handler();
    const changeAddressHandler = new ChangeAddressLogHandler();
    const spyEvent1Handler = jest.spyOn(createdLog1Handler, "handle");
    const spyEvent2Handler = jest.spyOn(createdLog2Handler, "handle");
    const spyEventChangeAddressHandler = jest.spyOn(changeAddressHandler, "handle");

    eventDispatcher.register("CustomerCreatedEvent", createdLog1Handler);
    eventDispatcher.register("CustomerCreatedEvent", createdLog2Handler);
    eventDispatcher.register("CustomerChangeAddressEvent", changeAddressHandler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][0]
    ).toMatchObject(createdLog1Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerCreatedEvent"][1]
    ).toMatchObject(createdLog2Handler);

    expect(
      eventDispatcher.getEventHandlers["CustomerChangeAddressEvent"][0]
    ).toMatchObject(changeAddressHandler);

    const event = new CustomerChangeAddressEvent({
      id: "1",
      name: "Customer Name",
      address: `Street, 0, City - 00000-000`
    });

    eventDispatcher.notify(new CustomerCreatedEvent(null));
    eventDispatcher.notify(event);

    expect(spyEventChangeAddressHandler).toHaveBeenCalled();
    expect(spyEvent1Handler).toHaveBeenCalled();
    expect(spyEvent2Handler).toHaveBeenCalled();
  })
});
