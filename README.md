# UmbracoForms.uCaptcha

A simple to use and integrate captcha plugin for Umbraco Forms which supports both hCaptcha and reCaptcha.

## Getting started

This package is supported on Umbraco 8.6+ and Umbraco Forms 8.4.1+.

### Installation

UmbracoForms.uCaptcha is available from [Our Umbraco](https://our.umbraco.com/packages/website-utilities/umbracoformshcaptcha/), [NuGet](https://www.nuget.org/packages/AaronSadler.HCaptcha/) or as a manual download directly from GitHub.

## Usage

This package adds both hCaptcha or reCaptcha to Umbraco Forms, configurable by the AppSettings in the Web.Config.

It has the ability to choose from the traditional checkbox or invisible options.

This is tested with **Umbraco V8.6.1** and **Umbraco Forms 8.4.1**

You can see a demo of it in action here:
[https://aaronsadler.uk/contact/](https://aaronsadler.uk/contact/)


## Getting Started

Before you begin you will need to get your API keys from your preferred provider, you can get these from the links below:

[hCaptcha](https://hCaptcha.com/?r=0d16470cad8d)

[reCaptcha](https://www.google.com/recaptcha/about/)

You will need to add the following AppSettings to your `Web.Config`

    <add key="hCaptchaSiteKey" value="YOUR SITE KEY" />
    <add key="hCaptchaSecretKey" value="YOUR SECRET KEY" />

To select the provider you will need to add the following AppSetting to your `Web.Config`
    
    <add key="uCaptchaProvider" value="PROVIDER" />

The choices are either `hCaptcha` or `reCaptcha`

### Contribution guidelines

To raise a new bug, create an issue on the GitHub repository. To fix a bug or add new features, fork the repository and send a pull request with your changes. Feel free to add ideas to the repository's issues list if you would to discuss anything related to the package.

## License

Copyright &copy; 2020 [Aaron Sadler](https://aaronsadler.uk/).

Licensed under the MIT License.
