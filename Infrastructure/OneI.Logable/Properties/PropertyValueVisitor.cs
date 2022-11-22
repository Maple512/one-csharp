namespace OneI.Logable.Properties;

using System;
using OneI.Logable.Properties.PropertyValues;

public abstract class PropertyValueVisitor<TState, TResult>
{
    protected virtual TResult Visit(TState state, PropertyValue propertyValue)
    {
        switch(propertyValue)
        {
            case ScalarPropertyValue scalar:
                return VisitScalar(state, scalar);
            case SequencePropertyValue sequence:
                return VisitSequence(state, sequence);
            case StructurePropertyValue structure:
                return VisitStructure(state, structure);
            case DicationaryPropertyValue dicationary:
                return VisitDicationary(state, dicationary);
            default:
                break;
        }

        throw new NotSupportedException("The value {propertyValue} is not of a type supported by this visitor.");
    }

    protected abstract TResult VisitStructure(TState state, StructurePropertyValue structure);

    protected abstract TResult VisitDicationary(TState state, DicationaryPropertyValue dicationary);

    protected abstract TResult VisitSequence(TState state, SequencePropertyValue sequence);

    protected abstract TResult VisitScalar(TState state, ScalarPropertyValue scalar);
}
