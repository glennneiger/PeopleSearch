export class KeysValueConverter {
  toView(obj) {
    if (!obj) {
      return null;
    }
    return Object.keys(obj);
  }
}
