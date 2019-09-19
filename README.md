# MVC Caching
MVC Caching for Kentico MVC, extending the DancingGoat Caching Implementation to allow for easy Caching both of Repository calls, database calls, and Output Caching.  Sets up and leverages Kentico's cache dependencies.

# Installation
1. Install the `MVCCaching.Base` NuGet Package to your MVC Site
1. Navigate to the [MVCCaching.Kentico](https://github.com/KenticoDevTrev/MVCCaching/tree/master/MVCCaching.Kentico) Folder
1. Copy and include the Infrastructure into your Kentico MVC Site
1. Add the code found in the [Global.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) Application_Start method into your application's Application_Start
1. Optionally add the [Globa.asax.cs](https://github.com/KenticoDevTrev/MVCCaching/blob/master/MVCCaching.Kentico/Global.asax.cs) GetVaryByCustomString method.

# Usage
1. See Documentation.md for full documentation

# Contributions, but fixes and License
Feel free to Fork and submit pull requests to contribute.

You can submit bugs through the issue list and i will get to them as soon as i can, unless you want to fix it yourself and submit a pull request!

Check the License.txt for License information

# Compatability
Can be used on any Kentico 12 SP site (hotfix 29 or above).
