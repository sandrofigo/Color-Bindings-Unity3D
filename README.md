# Color Bindings

[![openupm](https://img.shields.io/npm/v/com.sandrofigo.colorbindings?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.sandrofigo.colorbindings/)

A library for scriptable objects to store colors that are then automatically applied to Unity UI elements.

---

## Features

Currently the following basic Unity UI components are supported:

- `Image`
- `TextMeshProUGUI`
- `Button`

To add support for different components in your projects, use the class `ColorBindingBase`.

## Installation

- Install [OpenUPM CLI](https://github.com/openupm/openupm-cli#installation)
- Run the following command in your Unity project folder

  ```
  openupm add com.sandrofigo.colorbindings
  ```

## Usage

1. Create a bindable color via 'Assets -> Create -> UI -> Bindable Color'
2. Add one of the 'Color Binding' components on an object where a supported component is located e.g. next to an `Image` component
3. Drag the bindable color you created before into the color property on the color binding component

There is also a tool accessible via 'Tools -> Color Bindings Verification Tool' that can help you find components that are missing a color binding at runtime.

## Collaboration

Support this project with a ⭐️, report an issue or if you feel adventurous and would like to extend the functionality open a pull request.


