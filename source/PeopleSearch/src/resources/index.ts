import { FrameworkConfiguration } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    './attributes/element-resize-detector',
    './attributes/infinite-scroll',
    './binding-behaviors/dynamic-expression-binding-behavior',
    './value-converters/search-input',
    './value-converters/keys',
    './elements/loading-indicator',
    //'./elements/search-input/search-input',
  ]);
}
