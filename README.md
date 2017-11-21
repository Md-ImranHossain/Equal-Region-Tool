# Equal-Region-Tool
This tool divides a set of geographic features of the same class representing a geographic area in to several subsets/regions so that the sum of a numeric attribute of the features in each subset/region remains almost equal and the bounding polygon of regions do not overlap with each other. For example a set of polygons representing buildings in a residential area could divided into 5 parts so that the total floor space of each of the part (group of buildings) is equal. 

## Software Requirements
This tool has been developed using C# and ArcObjects library as an add-on for ArcGIS 10 or above. ArcMap has been chosen as a target platform for the implementation of the tool. 

## Installation
Double click on the "Multipatch solar radiation" ersi addin file and follow the instruction. After installation open the ArcMap then go to Customize>Add-in Manager>customize>Commands>Add-In Controls and drag the "My Button" from commands box to somewhere in the ArcMap menu bars. After that click on the "My Button" button

## Usage and Description of the GUI
A snap shot of the tool GUI is given in the following figure. At first the user should decide a method on which the dividation will take place. A supervised method means that the user should select seed point for each of the equal regions. For example if the user wants two equal regions as an output he/she should select two features manually and the region growing will in fact starts from that features on. Unsupervised methos on the other hand does not require any input from the user for seed point selection.

![](https://github.com/Md-ImranHossain/Equal-Region-Tool/blob/master/Equal%20Region/Images/Capture.PNG)

The user also have specijy the layer on which the tool will operate and more inportantly the attribute on with the equal region will be build. Finaly the user should also put a tolerence in meter and start the tool. A couple of outputs of the tool are given below.

![]()

