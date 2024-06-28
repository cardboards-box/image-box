using ImageBox.Ast;

namespace ImageBox.Tests;

[TestClass]
public class AstParserTests : TestSetup
{
    [TestMethod]
    public void Test()
    {
        var service = GetService<IAstParserService>();

        const string TEMPLATE = @"<template>
    <clear color=""transparent"" do />
    <rectangle 
        x=""0"" y=""0"" width=""100vw"" height=""100vh"" 
        radius=""10px"" fill=""#000"" 
        border-color=""#f50"" border-width=""5px"" />
    <image {primaryLogo} src=""./assets/logos/primary.png"" position=""cover"" />
    <text 
        {text} :font-size=""fontSize"" 
        color=""#fff"" vertical-align=""center"" 
        horizontal-align=""center"" value=""Your Trading Card""
    />
    <image {bottomLeft} src=""./assets/logos/bottom-left.png"" position=""cover"" />
    <image {bottomRight} src=""./assets/logos/bottom-right.png"" position=""cover"" />
</template>

<script>
import { unit, right, bottom, left } from 'drawing';

export default () => {
    const logoSize = unit('50vw');
    const fullHeight = unit('100vh');
    const fcSize = unit('15vw');

    const primaryLogo =  {
        width: logoSize,
        height: logoSize,
        x: logoSize / 2,
        y: (fullHeight / 2) - (logoSize / 2),
    };

    const textPos = {
        height: uint('1.2em'),
        width: primaryLogo.width,
        y: primaryLogo.y + primaryLogo.height + unit('10px'),
        x: primaryLogo.x,
    };

    const bottomLeft = {
        x: right('5vw'),
        y: bottom('5vw'),
        width: fcSize,
        height: fcSize,
    };

    const bottomRight = {
        x: left('5vw'),
        y: bottom('5vw'),
        width: fcSize,
        height: fcSize,
    }

    return {
        primaryLogo,
        text: textPos,
        bottomLeft,
        bottomRight,
        fontSize: '1em',
    };
}
</script>";

        var elements = service.ParseString(TEMPLATE, AstConfig.Default).ToArray();

        Assert.AreEqual(2, elements.Length, "Root Elements Count");

        var template = elements[0];
        Assert.AreEqual("template", template.Tag, "Template Element Name");
        Assert.AreEqual(6, template.Children.Length, "Template Elements Count");

        var clear = template.Children[0];
        Assert.AreEqual("clear", clear.Tag, "Clear Element Name");
        Assert.AreEqual(2, clear.Attributes.Length, "Clear Attributes Count");
        var color = clear.Attributes[0];
        Assert.AreEqual("color", color.Name, "Clear attribute color name");
        Assert.AreEqual("transparent", color.Value, "Clear attribute color value");
        var @do = clear.Attributes[1];
        Assert.AreEqual("do", @do.Name, "Clear attribute do name");
        Assert.AreEqual(AstAttributeType.BooleanTrue, @do.Type, "Clear attribute do type");

        var text = template.Children[3];
        Assert.AreEqual("text", text.Tag, "Text Element Name");
        Assert.AreEqual(0, text.Children.Length, "Text Elements Count");
        Assert.AreEqual(6, text.Attributes.Length, "Text Attributes Count");

        Assert.AreEqual(AstAttributeType.Spread, text.Attributes[0].Type, "Text Attributes {text} spread");
        Assert.AreEqual("text", text.Attributes[0].Name, "Text Attributes {text} value");
        Assert.IsNull(text.Attributes[0].Value, "Text Attributes {text} value");

        Assert.AreEqual(AstAttributeType.Bind, text.Attributes[1].Type, "Text Attributes :font-size bind");
        Assert.AreEqual("fontSize", text.Attributes[1].Value, "Text Attributes :font-size value");

        Assert.AreEqual(AstAttributeType.Value, text.Attributes[2].Type, "Text Attributes color value");
        Assert.AreEqual("#fff", text.Attributes[2].Value, "Text Attributes color value");

        var script = elements[1];
        Assert.AreEqual("script", script.Tag, "Script Element Name");
        Assert.AreEqual(0, script.Children.Length, "Script Elements Count");
        Assert.IsTrue(!string.IsNullOrWhiteSpace(script.Value), "Script Element Value");
    }
}
