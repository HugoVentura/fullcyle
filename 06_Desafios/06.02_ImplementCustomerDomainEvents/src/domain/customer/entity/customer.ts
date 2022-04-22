import EventDispatcher from "../../@shared/event/event-dispatcher";
import EventDispatcherInterface from "../../@shared/event/event-dispatcher.interface";
import CustomerChangeAddressEvent from "../event/customer-change-address.event";
import CustomerCreatedEvent from "../event/customer-created.event";
import ChangeAddressLogHandler from "../event/handler/change-address-log.handler";
import CreatedUserLog1Handler from "../event/handler/created-user-log-1.handler";
import CreatedUserLog2Handler from "../event/handler/created-user-log-2.handler";
import Address from "../value-object/address";

export default class Customer {
  private _id: string;
  private _name: string = "";
  private _address!: Address;
  private _active: boolean = false;
  private _rewardPoints: number = 0;
  private _eventDispatcher: EventDispatcherInterface;

  constructor(id: string, name: string) {
    this._id = id;
    this._name = name;
    this.validate();
    const changeAddressLogHandler = new ChangeAddressLogHandler();
    const createdLog1Handler = new CreatedUserLog1Handler();
    const createdLog2Handler = new CreatedUserLog2Handler();
    this._eventDispatcher = new EventDispatcher();
    this._eventDispatcher.register("CustomerCreatedEvent", createdLog1Handler);
    this._eventDispatcher.register("CustomerCreatedEvent", createdLog2Handler);
    this._eventDispatcher.register("CustomerChangeAddressEvent", changeAddressLogHandler);
    this._eventDispatcher.notify(new CustomerCreatedEvent(null));
  }

  get id(): string {
    return this._id;
  }

  get name(): string {
    return this._name;
  }

  get rewardPoints(): number {
    return this._rewardPoints;
  }

  validate() {
    if (this._id.length === 0) {
      throw new Error("Id is required");
    }
    if (this._name.length === 0) {
      throw new Error("Name is required");
    }
  }

  changeName(name: string) {
    this._name = name;
    this.validate();
  }

  get Address(): Address {
    return this._address;
  }
  
  changeAddress(address: Address) {
    this._address = address;
    const event = new CustomerChangeAddressEvent({
      id: this.id,
      name: this.name,
      address: `${address.street}, ${address.number}, ${address.city} - ${address.zip}`
    });

    this._eventDispatcher.notify(event);
  }

  isActive(): boolean {
    return this._active;
  }

  activate() {
    if (this._address === undefined) {
      throw new Error("Address is mandatory to activate a customer");
    }
    this._active = true;
  }

  deactivate() {
    this._active = false;
  }

  addRewardPoints(points: number) {
    this._rewardPoints += points;
  }

  set Address(address: Address) {
    this._address = address;
  }
}
