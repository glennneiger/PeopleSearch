import { autoinject, bindable, customAttribute } from 'aurelia-framework';

// Please note. As implemented, this attribute only works for scrollable elements,
// not for scrolling the entire page. It's simple to add that functionality if it's needed 
// eventually

@customAttribute('infinite-scroll')
@autoinject()
export class InfiniteScroll {
  isTicking = false;

  @bindable callback;
  @bindable scrollBuffer = 50;
  @bindable isActive = true;

  static ScrollEventName = 'scroll';

  constructor(private element: Element) {
  }

  attached() {
    this.element.addEventListener(InfiniteScroll.ScrollEventName, this.onScrollChange);
    this.onScrollChange();
  }

  detached() {
    this.element.removeEventListener(InfiniteScroll.ScrollEventName, this.onScrollChange);
  }

  scrollBufferChanged(buffer) {
    this.scrollBuffer = +buffer;
  }

  isActiveChanged(isActive) {
    this.isActive = (isActive === 'true');
  }

  onScrollChange = () => {
    const htmlElement = this.element as HTMLElement;

    if (!this.isActive || htmlElement.offsetParent == null || !this.callback ) {
      return false;
    }

    if (!this.isTicking) {
      window.requestAnimationFrame(() => {
        this.checkScrollPosition();
        this.isTicking = false;
      });
    }

    this.isTicking = true;
  }

  checkScrollPosition() {
    const htmlElement = this.element as HTMLElement;

    htmlElement.scrollHeight - htmlElement.clientHeight

    if (htmlElement.offsetParent && this.callback) {
      const elementScrollHeight = htmlElement.scrollHeight;
      const elementClientHeight = htmlElement.clientHeight;
      const elementScrollPosition = htmlElement.scrollTop;
      const isElementScrolledToBottom = (elementScrollPosition + this.scrollBuffer) >= (elementScrollHeight - elementClientHeight);

      if (isElementScrolledToBottom) {
        this.callback();
      }
    }
  }
}
