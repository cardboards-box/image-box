# image-box
A C# API for creating PNGs and GIFs from vue-like templates

## Installation
You can install the NuGet package:
```bash
PM> Install-Package ImageBox
```

## Setup
You can either add the ImageBox library to your project using either Dependency Injection or by using the `ImageBoxUtility` class.

### Usage
This is the preferred method of using this library as it gives you more control how the library is used. 
You can add the ImageBox library to your services like so:
```csharp
using ImageBox;

//Get these from your application
IConfiguration config;
IServiceCollection services;

//Register the ImageBox library with your services
services.AddImageBox(config);

...

using ImageBox;

//Fetch this from your services
IImageBoxService _imageBox;

//Get your template and image output paths
var template = "template.html";
var outputDir = "output";
var outputName = "some-image";

//Get the image context and services
var image = _imageBox.Create(template);	
var context = await _imageBox.LoadContext(image);

//Determine the output path
var outputExt = context.Animate ? "gif" : "png";
var outputPath = Path.Combine(outputDir, $"{outputName}.{outputExt}");

//Render the image
await _imageBox.Render(ouputPath, image);
```

You can see an example of this method in the `ImageBox.Cli` project.

### ImageBoxUtility
You can use the `ImageBoxUtility` class to create images like so:
```csharp
using ImageBox;

var inputPath = "template.html";
var outputPath = "output.gif";

var ib = ImageBoxUtility.From(inputPath);
await ib.Render(outputPath);
```


### Examples
You can check the various examples used in the `ImageBox.Cli` project.

More documentation will be added soon.