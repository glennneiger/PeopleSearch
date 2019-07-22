import { FrameworkConfiguration } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    './value-converters/search-input',
    './value-converters/keys',
    './elements/loading-indicator',
    //'./elements/search-input/search-input',
  ]);
}
