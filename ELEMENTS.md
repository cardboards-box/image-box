# Table of Contents

| Type | Name | Description |
| ---- | ---- | ----------- |
| [Element](#element-cache) | `cache` | Element to allow for caching resources to use across multiple renders |
| [Element](#element-script) | `script` | Represents a script that can be executed to calculate contexts |
| [Element](#element-template) | `template` | Represents the root of a template |
| [Element](#element-animation-bezier) | `animation-bezier` | Renders the children of the element with a Bezier animation |
| [Element](#element-case) | `case` | The case statement for the [SwitchDir](#element-switch) directive |
| [Element](#element-circle) | `circle` | Represents a circle that can be filled or bordered |
| [Element](#element-clear) | `clear` | The clear element |
| [Element](#element-default) | `default` | The default case for a switch statement |
| [Element](#element-foreach) | `foreach` | Represents a for-each directive |
| [Element](#element-if) | `if` | If directive for templates |
| [Element](#element-image) | `image` | Represents an image that can be drawn to the image |
| [Element](#element-line) | `line` | Represents a line that can be filled or bordered |
| [Element](#element-range) | `range` | Represents a for directive |
| [Element](#element-rectangle) | `rectangle` | Represents a rectangle that can be filled or bordered |
| [Element](#element-switch) | `switch` | Switch directive for templates |
| [Element](#element-text) | `text` | Represents text to be drawn to the image |
| [Element](#element-font-family) | `font-family` | Element to allow for importing of custom fonts |
| [Element](#element-point) | `point` | Represents a point in the render context |
| [Element](#element-remote-resource) | `remote-resource` | Represents a remote resource that should be cached |
| [Type](#type-System-Object-array) | `Object[]` |  |
| [Type](#type-System-String) | `String` |  |
| [Type](#type-System-Double) | `Double` | Represents a double-precision floating-point number. |
| [Type](#type-System-Boolean) | `Boolean` | Represents a boolean (`true` or `false`) value. |
| [Type](#type-System-Object) | `Object` |  |
| [Type](#type-SixLabors-Fonts-VerticalAlignment) | `VerticalAlignment` | Vertical alignment modes. |
| [Type](#type-SixLabors-Fonts-HorizontalAlignment) | `HorizontalAlignment` | Horizontal alignment modes. |
| [Type](#type-SixLabors-Fonts-TextAlignment) | `TextAlignment` | Text alignment modes. |
| [Type](#type-ImageBox-Drawing-OriginType) | `OriginType` | Dictates where the origin of an object is |
| [Type](#type-ImageBox-Core-SizeUnits-SizeUnit) | `SizeUnit` | Unit of measurement (ex: 100vw, 50vh, 10px, 40%, 1cm, 2in, 2pc, 43pt, 1em, 1mm, 1q, 3rp) |
| [Type](#type-SixLabors-Fonts-FontStyle) | `FontStyle` | The font styles |
| [Type](#type-SixLabors-Fonts-WordBreaking) | `WordBreaking` | Defines modes to determine when line breaks should appear when words overflow their content box. |
| [Type](#type-ImageBox-Core-IOPath-IOPath) | `IOPath` | A URI path - can be remote, local, or use fancy scheme (https, ftp, etc) |
| [Type](#type-ImageBox-Drawing-Models-Bezier-BezierType) | `BezierType` | The type of Bezier curve to use |
| [Type](#type-ImageBox-Drawing-Models-Bezier-EasingType) | `EasingType` | The type of easing function to use |
| [Type](#type-ImageBox-Core-TimeUnits-TimeUnit) | `TimeUnit` | Unit of time. (ex: 20ms, 3.2s, 1m, 2h) |
| [Type](#type-System-UInt16) | `UInt16` |  |



# Elements / Tags
<a name="element-cache"></a>
## Element: `<cache>`
Element to allow for caching resources to use across multiple renders

**Example**:<br>
```html
<cache >
  <!-- CHILDREN HERE -->
</cache>
```

<a name="element-script"></a>
## Element: `<script>`
Represents a script that can be executed to calculate contexts

**Example**:<br>
```html
<script 
  setup="" 
  name="" 
  src="" 
>
  <!-- TEXT VALUE HERE -->
</script>
```

### Attributes

<a name="attribute-script-setup"></a>
__*setup*__: Whether or not the script is the entry point to the image<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-script-name"></a>
__*name*__: The name of the module to use when injecting into other scripts<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.
You can use the following aliases: `module`.

<a name="attribute-script-src"></a>
__*src*__: Where to look to populate this script<br>
Type: [ImageBox.Core.IOPath.IOPath](#type-ImageBox-Core-IOPath-IOPath).
This attribute is optional.
This attribute cannot be bound to a runtime variable.
You can use the following aliases: `source`, `path`.


<a name="element-template"></a>
## Element: `<template>`
Represents the root of a template

**Example**:<br>
```html
<template 
  width="" 
  height="" 
  font-size="" 
  font-family="" 
  animate="" 
  animate-duration="" 
  animate-fps="" 
  animate-repeat="" 
>
  <!-- CHILDREN HERE -->
</template>
```

### Attributes

<a name="attribute-template-width"></a>
__*width*__: The width of the image<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-height"></a>
__*height*__: The height of the image<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-font-size"></a>
__*font-size*__: The default size of the font for the image<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-font-family"></a>
__*font-family*__: The default font family to use for the image<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-animate"></a>
__*animate*__: Whether or not to animate the image<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-animate-duration"></a>
__*animate-duration*__: The duration to animate the image<br>
Type: [ImageBox.Core.TimeUnits.TimeUnit](#type-ImageBox-Core-TimeUnits-TimeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-animate-fps"></a>
__*animate-fps*__: The number of frames per second for the animation<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-template-animate-repeat"></a>
__*animate-repeat*__: How many times to repeat the gif<br>
Type: [System.UInt16](#type-System-UInt16).
This attribute is optional.
This attribute cannot be bound to a runtime variable.
*Remarks*: 0 is repeat forever, x is repeat number of times


<a name="element-animation-bezier"></a>
## Element: `<animation-bezier>`
Renders the children of the element with a Bezier animation

**Example**:<br>
```html
<animation-bezier 
  type="" 
  easing="" 
  x="" 
  y="" 
  width="" 
  height="" 
>
  <!-- CHILDREN HERE -->
</animation-bezier>
```

### Attributes

<a name="attribute-animation-bezier-type"></a>
__*type*__: The type of Bezier curve to use<br>
Type: [ImageBox.Drawing.Models.Bezier.BezierType](#type-ImageBox-Drawing-Models-Bezier-BezierType).
This attribute is optional.
This attribute can be bound to a runtime variable.
You can use the following aliases: `interpolation`.

<a name="attribute-animation-bezier-easing"></a>
__*easing*__: The easing function to use<br>
Type: [ImageBox.Drawing.Models.Bezier.EasingType](#type-ImageBox-Drawing-Models-Bezier-EasingType).
This attribute is optional.
This attribute can be bound to a runtime variable.
You can use the following aliases: `timing`.

<a name="attribute-animation-bezier-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-animation-bezier-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-animation-bezier-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-animation-bezier-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-case"></a>
## Element: `<case>`
The case statement for the [SwitchDir](#element-switch) directive

**Example**:<br>
```html
<case when="" >
  <!-- CHILDREN HERE -->
</case>
```

### Attributes

<a name="attribute-case-when"></a>
__*when*__: The value to compare against<br>
Type: [System.Object](#type-System-Object).
This attribute is optional.
This attribute can be bound to a runtime variable.
You can use the following aliases: `con`, `condition`, `value`.


<a name="element-circle"></a>
## Element: `<circle>`
Represents a circle that can be filled or bordered

**Example**:<br>
```html
<circle 
  color="" 
  border-color="" 
  border-width="" 
  x="" 
  y="" 
  width="" 
  height="" 
>
  <!-- CHILDREN HERE -->
</circle>
```

### Attributes

<a name="attribute-circle-color"></a>
__*color*__: The color to fill with<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-border-color"></a>
__*border-color*__: The color of the border of the rectangle<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-border-width"></a>
__*border-width*__: The width of the border of the rectangle<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-circle-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-clear"></a>
## Element: `<clear>`
The clear element

**Example**:<br>
```html
<clear color="" />
```

### Attributes

<a name="attribute-clear-color"></a>
__*color*__: The color to clear with<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-default"></a>
## Element: `<default>`
The default case for a switch statement

**Example**:<br>
```html
<default >
  <!-- CHILDREN HERE -->
</default>
```

<a name="element-foreach"></a>
## Element: `<foreach>`
Represents a for-each directive

**Example**:<br>
```html
<foreach each="" let="" >
  <!-- CHILDREN HERE -->
</foreach>
```

### Attributes

<a name="attribute-foreach-each"></a>
__*each*__: Iterate through each of the values<br>
Type: [System.Object[]](#type-System-Object-array).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-foreach-let"></a>
__*let*__: What to name the value in the children template contexts<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.


<a name="element-if"></a>
## Element: `<if>`
If directive for templates

**Example**:<br>
```html
<if condition="" >
  <!-- CHILDREN HERE -->
</if>
```

### Attributes

<a name="attribute-if-condition"></a>
__*condition*__: The condition for the if statement<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute can be bound to a runtime variable.
You can use the following aliases: `con`.


<a name="element-image"></a>
## Element: `<image>`
Represents an image that can be drawn to the image

**Example**:<br>
```html
<image 
  src="" 
  rotate="" 
  flip-vertical="" 
  flip-horizontal="" 
  user-agent="" 
  accepts="" 
  should-cache="" 
  x="" 
  y="" 
  width="" 
  height="" 
/>
```

### Attributes

<a name="attribute-image-src"></a>
__*src*__: The images source<br>
Type: [ImageBox.Core.IOPath.IOPath](#type-ImageBox-Core-IOPath-IOPath).
This attribute is **Required**.
This attribute can be bound to a runtime variable.
You can use the following aliases: `source`.

<a name="attribute-image-rotate"></a>
__*rotate*__: The number of degrees to rotate the image before rendering<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-flip-vertical"></a>
__*flip-vertical*__: Whether to flip the image vertically or not<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-flip-horizontal"></a>
__*flip-horizontal*__: Whether to flip the image horizontally or not<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-user-agent"></a>
__*user-agent*__: The optional User-Agent header for fetching the file<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-accepts"></a>
__*accepts*__: The optional Accepts header for fetching the file<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-should-cache"></a>
__*should-cache*__: Indicates whether or not the file should be cached locally<br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-image-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-line"></a>
## Element: `<line>`
Represents a line that can be filled or bordered

**Example**:<br>
```html
<line 
  color="" 
  border-color="" 
  border-width="" 
  x="" 
  y="" 
  width="" 
  height="" 
>
  <!-- CHILDREN HERE -->
</line>
```

### Attributes

<a name="attribute-line-color"></a>
__*color*__: The color to fill with<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-border-color"></a>
__*border-color*__: The color of the border of the rectangle<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-border-width"></a>
__*border-width*__: The width of the border of the rectangle<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-line-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-range"></a>
## Element: `<range>`
Represents a for directive

**Example**:<br>
```html
<range 
  start="" 
  end="" 
  step="" 
  let="" 
>
  <!-- CHILDREN HERE -->
</range>
```

### Attributes

<a name="attribute-range-start"></a>
__*start*__: The start of value<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-range-end"></a>
__*end*__: The end value of the loop<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-range-step"></a>
__*step*__: The step to increment each iteration by<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-range-let"></a>
__*let*__: What to name the value in the children template contexts<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.


<a name="element-rectangle"></a>
## Element: `<rectangle>`
Represents a rectangle that can be filled or bordered

**Example**:<br>
```html
<rectangle 
  radius="" 
  color="" 
  border-color="" 
  border-width="" 
  x="" 
  y="" 
  width="" 
  height="" 
>
  <!-- CHILDREN HERE -->
</rectangle>
```

### Attributes

<a name="attribute-rectangle-radius"></a>
__*radius*__: The radius of the curved corners<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-color"></a>
__*color*__: The color to fill with<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-border-color"></a>
__*border-color*__: The color of the border of the rectangle<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-border-width"></a>
__*border-width*__: The width of the border of the rectangle<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-rectangle-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-switch"></a>
## Element: `<switch>`
Switch directive for templates

**Example**:<br>
```html
<switch value="" >
  <!-- CHILDREN HERE -->
</switch>
```

### Attributes

<a name="attribute-switch-value"></a>
__*value*__: The value to switch on<br>
Type: [System.Object](#type-System-Object).
This attribute is optional.
This attribute can be bound to a runtime variable.
You can use the following aliases: `target`, `condition`, `con`.


<a name="element-text"></a>
## Element: `<text>`
Represents text to be drawn to the image

**Example**:<br>
```html
<text 
  value="" 
  color="" 
  align-vertical="" 
  align-horizontal="" 
  align-text="" 
  rotate="" 
  rotate-origin-type="" 
  rotate-origin-x="" 
  rotate-origin-y="" 
  origin-type="" 
  origin-x="" 
  origin-y="" 
  font-size="" 
  font-family="" 
  font-style="" 
  auto-font-size="" 
  auto-font-size-padding="" 
  auto-font-size-word-breaking="" 
  x="" 
  y="" 
  width="" 
  height="" 
/>
```

### Attributes

<a name="attribute-text-value"></a>
__*value*__: The value of the text to draw to the image<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-color"></a>
__*color*__: The color to fill with<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-align-vertical"></a>
__*align-vertical*__: Where to align the text vertically in the rectangle<br>
Type: [SixLabors.Fonts.VerticalAlignment](#type-SixLabors-Fonts-VerticalAlignment).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-align-horizontal"></a>
__*align-horizontal*__: Where to align the text horizontally in the rectangle<br>
Type: [SixLabors.Fonts.HorizontalAlignment](#type-SixLabors-Fonts-HorizontalAlignment).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-align-text"></a>
__*align-text*__: How to align the text within the rectangle<br>
Type: [SixLabors.Fonts.TextAlignment](#type-SixLabors-Fonts-TextAlignment).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-rotate"></a>
__*rotate*__: The number of degrees to rotate the image before rendering<br>
Type: [System.Double](#type-System-Double).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-rotate-origin-type"></a>
__*rotate-origin-type*__: How to determine the origin point of the text rotation within the current box<br>
Type: [ImageBox.Drawing.OriginType](#type-ImageBox-Drawing-OriginType).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Ignored if both [RotateOriginX](#attribute-text-rotate-origin-x) and [RotateOriginY](#attribute-text-rotate-origin-y) are set

<a name="attribute-text-rotate-origin-x"></a>
__*rotate-origin-x*__: The x coordinate of the origin point of the text rotation<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Requires [RotateOriginY](#attribute-text-rotate-origin-y) to be set as well

<a name="attribute-text-rotate-origin-y"></a>
__*rotate-origin-y*__: The y coordinate of the origin point of the text rotation<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Requires [RotateOriginX](#attribute-text-rotate-origin-x) to be set as well

<a name="attribute-text-origin-type"></a>
__*origin-type*__: How to determine the origin point of the text within the current box<br>
Type: [ImageBox.Drawing.OriginType](#type-ImageBox-Drawing-OriginType).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Ignored if both [OriginX](#attribute-text-origin-x) and [OriginY](#attribute-text-origin-y) are set

<a name="attribute-text-origin-x"></a>
__*origin-x*__: The x coordinate of the origin point<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Requires [OriginY](#attribute-text-origin-y) to be set as well

<a name="attribute-text-origin-y"></a>
__*origin-y*__: The y coordinate of the origin point<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.
*Remarks*: Requires [OriginX](#attribute-text-origin-x) to be set as well

<a name="attribute-text-font-size"></a>
__*font-size*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-font-family"></a>
__*font-family*__: <br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-font-style"></a>
__*font-style*__: <br>
Type: [SixLabors.Fonts.FontStyle](#type-SixLabors-Fonts-FontStyle).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-auto-font-size"></a>
__*auto-font-size*__: <br>
Type: [System.Boolean](#type-System-Boolean).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-auto-font-size-padding"></a>
__*auto-font-size-padding*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-auto-font-size-word-breaking"></a>
__*auto-font-size-word-breaking*__: <br>
Type: [SixLabors.Fonts.WordBreaking](#type-SixLabors-Fonts-WordBreaking).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-x"></a>
__*x*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-y"></a>
__*y*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-width"></a>
__*width*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-text-height"></a>
__*height*__: <br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-font-family"></a>
## Element: `<font-family>`
Element to allow for importing of custom fonts

**Example**:<br>
```html
<font-family name="" src="" />
```

### Attributes

<a name="attribute-font-family-name"></a>
__*name*__: The name of the font family<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-font-family-src"></a>
__*src*__: Where to find the font file<br>
Type: [ImageBox.Core.IOPath.IOPath](#type-ImageBox-Core-IOPath-IOPath).
This attribute is optional.
This attribute cannot be bound to a runtime variable.
You can use the following aliases: `source`, `path`.


<a name="element-point"></a>
## Element: `<point>`
Represents a point in the render context

**Example**:<br>
```html
<point x="" y="" />
```

### Attributes

<a name="attribute-point-x"></a>
__*x*__: The X offset<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.

<a name="attribute-point-y"></a>
__*y*__: The Y offset<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute can be bound to a runtime variable.


<a name="element-remote-resource"></a>
## Element: `<remote-resource>`
Represents a remote resource that should be cached

**Remarks**:<br>
Not implemented yet

**Example**:<br>
```html
<remote-resource 
  key="" 
  src="" 
  width="" 
  height="" 
/>
```

### Attributes

<a name="attribute-remote-resource-key"></a>
__*key*__: The key to cache with for retrieval<br>
Type: [System.String](#type-System-String).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-remote-resource-src"></a>
__*src*__: The remote source<br>
Type: [ImageBox.Core.IOPath.IOPath](#type-ImageBox-Core-IOPath-IOPath).
This attribute is optional.
This attribute cannot be bound to a runtime variable.
You can use the following aliases: `source`, `path`.

<a name="attribute-remote-resource-width"></a>
__*width*__: The width to cache the value as (if it's an image)<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.

<a name="attribute-remote-resource-height"></a>
__*height*__: The height to cache the value as (if it's an image)<br>
Type: [ImageBox.Core.SizeUnits.SizeUnit](#type-ImageBox-Core-SizeUnits-SizeUnit).
This attribute is optional.
This attribute cannot be bound to a runtime variable.


# Types
<a name="type-System-Object-array"></a>
### Type: `Object[]`
Full Name: System.Object[]<br>
<a name="type-System-String"></a>
### Type: `String`
Full Name: System.String<br>
<a name="type-System-Double"></a>
### Type: `Double`
Full Name: System.Double<br>
Description:
Represents a double-precision floating-point number.
<a name="type-System-Boolean"></a>
### Type: `Boolean`
Full Name: System.Boolean<br>
Description:
Represents a boolean (`true` or `false`) value.
<a name="type-System-Object"></a>
### Type: `Object`
Full Name: System.Object<br>
<a name="type-SixLabors-Fonts-VerticalAlignment"></a>
### Type: `VerticalAlignment`
Full Name: SixLabors.Fonts.VerticalAlignment<br>
Description:
Vertical alignment modes.
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Top` | Aligns downward from the top. | 0 |
| `Center` | Aligns text up and down from the middle. | 1 |
| `Bottom` | Aligns text upwards from the bottom | 2 |

<a name="type-SixLabors-Fonts-HorizontalAlignment"></a>
### Type: `HorizontalAlignment`
Full Name: SixLabors.Fonts.HorizontalAlignment<br>
Description:
Horizontal alignment modes.
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Left` | Aligns text from the left. | 0 |
| `Right` | Aligns text from the right. | 1 |
| `Center` | Aligns text from the center. | 2 |

<a name="type-SixLabors-Fonts-TextAlignment"></a>
### Type: `TextAlignment`
Full Name: SixLabors.Fonts.TextAlignment<br>
Description:
Text alignment modes.
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Start` | Aligns text from the left or top when the text direction is `LeftToRight` and from the right or bottom when the text direction is `RightToLeft`. | 0 |
| `End` | Aligns text from the right or bottom when the text direction is `LeftToRight` and from the left or top when the text direction is `RightToLeft`. | 1 |
| `Center` | Aligns text from the center. | 2 |

<a name="type-ImageBox-Drawing-OriginType"></a>
### Type: `OriginType`
Full Name: ImageBox.Drawing.OriginType<br>
Description:
Dictates where the origin of an object is
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `TopLeft` | Top left corner | 0 |
| `TopCenter` | Top center | 1 |
| `TopRight` | Top right corner | 2 |
| `CenterLeft` | Center left | 3 |
| `Center` | Center | 4 |
| `CenterRight` | Center right | 5 |
| `BottomLeft` | Bottom left corner | 6 |
| `BottomCenter` | Bottom center | 7 |
| `BottomRight` | Bottom right corner | 8 |

<a name="type-ImageBox-Core-SizeUnits-SizeUnit"></a>
### Type: `SizeUnit`
Full Name: ImageBox.Core.SizeUnits.SizeUnit<br>
Description:
Unit of measurement (ex: 100vw, 50vh, 10px, 40%, 1cm, 2in, 2pc, 43pt, 1em, 1mm, 1q, 3rp)
<a name="type-SixLabors-Fonts-FontStyle"></a>
### Type: `FontStyle`
Full Name: SixLabors.Fonts.FontStyle<br>
Description:
The font styles
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Regular` | Regular | 0 |
| `Bold` | Bold | 1 |
| `Italic` | Italic | 2 |
| `BoldItalic` | Bold and Italic | 3 |

<a name="type-SixLabors-Fonts-WordBreaking"></a>
### Type: `WordBreaking`
Full Name: SixLabors.Fonts.WordBreaking<br>
Description:
Defines modes to determine when line breaks should appear when words overflow
their content box.
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Standard` | Use the default line break rule. | 0 |
| `BreakAll` | To prevent overflow, word breaks should be inserted between any two characters (excluding Chinese/Japanese/Korean text). | 1 |
| `KeepAll` | Word breaks should not be used for Chinese/Japanese/Korean (CJK) text. Non-CJK text behavior is the same as for `Standard` | 2 |
| `BreakWord` | Uses a combination of `Standard` and `BreakAll` rules in that order. | 3 |

<a name="type-ImageBox-Core-IOPath-IOPath"></a>
### Type: `IOPath`
Full Name: ImageBox.Core.IOPath.IOPath<br>
Description:
A URI path - can be remote, local, or use fancy scheme (https, ftp, etc)
<a name="type-ImageBox-Drawing-Models-Bezier-BezierType"></a>
### Type: `BezierType`
Full Name: ImageBox.Drawing.Models.Bezier.BezierType<br>
Description:
The type of Bezier curve to use
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `Linear` | Linear interpolation | 0 |
| `Quadratic` | Quadratic interpolation | 1 |
| `Cubic` | Cubic interpolation | 2 |

<a name="type-ImageBox-Drawing-Models-Bezier-EasingType"></a>
### Type: `EasingType`
Full Name: ImageBox.Drawing.Models.Bezier.EasingType<br>
Description:
The type of easing function to use
**Enum Options**:<br>
| Name | Description | Value |
| ---- | ----------- | ----- |
| `In` | Ease-in | 0 |
| `Out` | Ease-out | 1 |
| `InOut` | Ease-in-out | 2 |

<a name="type-ImageBox-Core-TimeUnits-TimeUnit"></a>
### Type: `TimeUnit`
Full Name: ImageBox.Core.TimeUnits.TimeUnit<br>
Description:
Unit of time. (ex: 20ms, 3.2s, 1m, 2h)
<a name="type-System-UInt16"></a>
### Type: `UInt16`
Full Name: System.UInt16<br>
