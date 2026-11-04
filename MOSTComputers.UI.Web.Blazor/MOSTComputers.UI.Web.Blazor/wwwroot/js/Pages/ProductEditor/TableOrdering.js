export class SingleActiveTypeOrderingState {
    _orderTypes;
    _currentActiveOrdering;

    constructor(orderTypes, currentActiveOrdering = null) {
        this._orderTypes = orderTypes;
        this._currentActiveOrdering = currentActiveOrdering;
    }

    changeActiveOrder(type, option) {

        const options = this._orderTypes.get(type);

        if (!options.includes(option)) {

            throw new Error("Option does not exist");
        }

        this._currentActiveOrdering = new SingleActiveTypeActiveOrderingState(type, option);
    }

    get activeOrder() { return this._currentActiveOrdering; }

    clearActiveOrder() {
        this._currentActiveOrdering = null;
    }
}

export class SingleActiveTypeActiveOrderingState {

    constructor(type, option) {
        this._type = type;
        this._option = option;
    }

    get type() { return this._type; }
    get option() { return this._option; }
}
