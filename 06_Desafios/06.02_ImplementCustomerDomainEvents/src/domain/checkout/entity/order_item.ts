export default class OrderItem {
  private _id: string;
  private _productId: string;
  private _name: string;
  private _unitaryPrice: number;
  private _quantity: number;

  constructor(id: string, name: string, unitaryPrice: number, productId: string, quantity: number) {
    this._id = id;
    this._name = name;
    this._unitaryPrice = unitaryPrice;
    this._productId = productId;
    this._quantity = quantity;
  }

  get id(): string {
    return this._id;
  }

  get name(): string {
    return this._name;
  }

  get productId(): string {
    return this._productId;
  }

  get quantity(): number {
    return this._quantity;
  }

  get unitaryPrice(): number {
    return this._unitaryPrice;
  }

  get total(): number {
    return this.unitaryPrice * this._quantity;
  }
}
