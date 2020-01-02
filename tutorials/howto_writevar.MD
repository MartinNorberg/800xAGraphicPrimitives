It was asked how we can write property values back to PG2. 

Short answer: we can't. PG2 properties are a one-way street. The read-functionallity of TransferValue is used in Graphic Builder only.

But: we can use a property reference to transfer a value back to PG2

```C#
new PropertyDesc("TargetYPos", PropertyRefType.Singleton, 18, "Output Y value.", "Interface"),
```
The proprty type have to be **PropertyRefType.Singleton**

```C#
private PropertyRefValue TargetYPos = null;
```
For storing the variable reference locally

```C#
this.TargetYPos = PropertyRefValue.GetLeftSideValue(other.TargetYPos,elementView);
```
In the clone-constructor, we have to use this to copy the object. Direct copy of the object reference does not work!

```C#
case 18:
    accessor.TransferPropertyReferenceValue(writeOperation, accessIndex, ref TargetYPos);
    break;
```
The TransferValue operaton is also different

```C#
elementView.WriteProperty(new RealVariantValue(Position.Y), TargetYPos);
```
The value writing itself
