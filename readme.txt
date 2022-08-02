TabStrips: A TabControl in the Visual Studio 2005 way!

Besides all the new features of Visual Studio, there is a distinct TabControl with a new style like no other. The code here tries to build up a control which behaves like the tab pages of VS.NET 2005.

Building a TabControl consists of building two controls: a TabControl itself, which is a container control, but only accepts TabPages as a containing control. This could be done by assigning a Designer class to the TabControl. Now, TabPage is a container control too, but can accept all kinds of controls hosted in it.

The Visual Studio TabControl slightly differs from a normal tab control, because it only shows as much TabPages as fits the width of the control. All the other TabPages are available via a context menu appearing by clicking the glyph icon. Selecting a TabPage from the menu brings it to position 0 of the TabPageCollection, so that it appears as the first TabPage.

http://www.codeproject.com/Articles/13902/TabStrips-A-TabControl-in-the-Visual-Studio-2005-w

PageControl:
https://github.com/oozcitak/PagedControl
TabControl:
https://github.com/oozcitak/TabControl

CustomTab:
This project is just a modified version of Painting Your Own Tabs - Second Edition made by The Man from U.N.C.L.E.
https://www.codeproject.com/Articles/91387/Painting-Your-Own-Tabs-Second-Edition-2

By no means am I the creator of this source code.
TabStrips 原始控件库很久没有更新了，这里是为了适应自己的项目做了一些改进与美化
<img src="https://github.com/RainsSoft/TabStrips/blob/master/demo.png">
