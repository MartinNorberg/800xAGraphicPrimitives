# How to create 800xA PG2 Graphic Primitives in C#
## Why I should use C#?
Before we start to explain *how* to create Graphic Primitives in C# we should clarify *why* we do so.

PG2 is a well done, very powerful graphic system made by ABB. We can display nearly anything with buildin functions.

But there are rare cases where there are still limitations. Want some examples?

- It is not possible to build "loops" with variables.
- Is is not possible to get properties from Objects to variables. (To calculate the position of one Object from multiple others)
- It is not possible to use advanced input controls (like Mouse Wheel...)
- It is not possible to connect to external resources like SQL servers, etc.
- It is not possible to Control Windows resources from inside the Graphics.

All this is possible (and many more...) with C#.

## Nice, but whats the drawbacks?
There are many reasons for *not* using this possibility.

- There is no support from ABB. If something is going ugly, ABB can blame your Graphic Primitive for everything.
- You have to test and upgrade your Graphic Primitive on every 800xA update. ABB does not promise stable (not changing) interfaces for the PG2 implementation.
- You have to deploy your Graphic Primitive by yourself. There is no System Installer support.
- Graphic Primitives have to be digital signed. Of corse we do not have this signature key, so have two options
    - Enable Signature Bypassing for your Primitive. This is a possible **security weakness.**
    - Ask ABB for signing your Primitive. I have no idea if they do this at all. Not my cup of tea, thats busyness.

## Creating a Graphic Primitive
[Implementation](implementation.MD)
