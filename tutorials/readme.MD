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
- Create a new C# class library (.net Framework) project. Chose the .net framework version depending on your 800xA version. You can also use higher Versions, but in this case they have to be installed on the target system.
    - 800xA V5.1 uses .net 3.5
    - 800xA 6.0/6.1 uses .net 4.0
- Get the the public strong name signature key. You can get it from the example project, or extract it from some other graphic primitive assembly. For extraction we are using the dotnet strong name tool. More informations here: https://msdn.microsoft.com/en-us/library/k5b5tt23%28VS.80%29.aspx
This is the complete command:
'sn -e "c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\GraphicPrimitives\ABB.xA.ProcessGraphics.AspectViewPrimitives.dll" "PG2Key.snk"
- Add the signature key to your project.
    - go to the project properties... signing
    - check "Sign the assembly", browse for the PG2Key.snk, check "Delay Sign only" too.
- Add a reference to the **GraphicsKernel.dll**, you can usually find this file in **c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\**.
- Implement the Class
