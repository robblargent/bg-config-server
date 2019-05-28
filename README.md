# bg-config-server

This repo contains an application used to test the Steeltoe 2.2 Configuration libraries in a blue/green deployment scenario.

This application references the configuration file(s) located at https://github.com/robblargent/bg-config-server-configs .

## Synopsis

To support blue/green deployments on PCF our CI/CD pipelines push an application to the same PCF foundation/organization/space with 2 different names.

For example, given an application named  `bg-config-server`:

* `bg-config-server` - the **live** application currently taking traffic with a route of `https://bg-config-server.cfapps.io`

* `bg-config-server-v2-0-0` - the **new**, not live, with a route of `https://bg-config-server-v2-0-0.cfapps.io`

The only difference between these applications is the name of the application in their respective `manifest.yml` file.

The `spring:application:name` entry in `appsettings.json` is set to `bg-config-server`.

Both applications need the ability to consume configuration from the `bg-config-server.yml` file.

## Pre-requisites for testing

* Have access to a PCF Foundation to which application can be pushed

* A Config Server service named `config-server` configured to pull configuration files from https://github.com/robblargent/bg-config-server-configs
  
## Steps to reproduce

1. execute `git clone https://github.com/robblargent/bg-config-server.git`

2. execute `cf login -a {api url to your PCF foundation}
   * For example: `cf login -a api.run.pivotal.io`)

3. execute `publish-deploy-linux-pcf.sh`
   When finished there should be 2 applications available in PCF:
   * `bg-config-server`
   * `bg-config-server-v2-0-0`

4. Navigate to https://{bg-config-server route}/configuration
   * https://bg-config-server.cfapps.io/configuration
     * Notice key = "applicationName" has value = "bg-config-server"
     * Notice key = "appSettings:mySetting" has value = "This is a value from appsettings.json"
     * Notice key = "FROM_ENVIRONMENT" has value = "This is a \"bg-config-server-v2-0-0\" environment variable value"
     * Notice key = "spring:application:name" has value = "bg-config-server"
     * Notice key = "vcap:application:application_name" has value = "bg-config-server"
     * Notice key = "fromConfigServer1" has value = "A setting"
     * Notice key = "fromConfigServer2" has value = "Another setting"
     * Notice key = "fromConfigServer3" has value = "True"
     * Notice key = "fromConfigServer4" has value = "42"

5. Navigate to https://{bg-config-server-v2-0-0 route}/configuration
   * https://bg-config-server-v2-0-0.cfapps.io/configuration
     * Notice key = "applicationName" has value = "bg-config-server"
     * Notice key = "appSettings:mySetting" has value = "This is a value from appsettings.json"
     * Notice key = "FROM_ENVIRONMENT" has value = "This is a \"bg-config-server-v2-0-0\" environment variable value"
     * Notice key = "spring:application:name" has value = "bg-config-server-v2-0-0"
     * Notice key = "vcap:application:application_name" has value = "bg-config-server-v2-0-0"
     * Notice key = "fromConfigServer1" **DOES NOT EXIST**
     * Notice key = "fromConfigServer2" **DOES NOT EXIST**
     * Notice key = "fromConfigServer3" **DOES NOT EXIST**
     * Notice key = "fromConfigServer4" **DOES NOT EXIST**

## Issue

The value of `spring:application:name` for the `bg-config-server-v2-0-0` application **does not** contain the value from the `appsettings.json` file pushed with the application.  Rather, it has been overridden with the application name from `manifest-bg-config-server-v2-0-0.yml` and, therefore, from `vcap:application:application_name`.

Due to this issue the `bg-config-server-v2-0-0` application is unable to retrieve configuration via Config Server from `bg-config-server.yml`.

## Expected Results

The `spring:application:name`, if it exists in `appsetings.json`, should not be replaced with the name of the application from the manifest and should be used as the name of the application for which configuration will be retrieved via Config Server.
