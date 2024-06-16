﻿using EStimLibrary.Core;
using EStimLibrary.Core.SpatialModel;


namespace EStimLibrary.Extensions.SpatialModel.StringHierarchy;


//public class StringHierarchySpecFactory<S> : IFactory<S>
//    where S : StringHierarchySpec
public class StringHierarchyLocationFactory :
    IFactory<ILocation>
{
    private readonly StringHierarchyRegion _baseRegion;

    public string HelpMsg { get; init; }
    public Dictionary<string, IDataLimits> ParamLimits { get; init; }

    public StringHierarchyLocationFactory(
        StringHierarchyRegion baseModelRegion)
    {
        this._baseRegion = baseModelRegion;

        this.HelpMsg = $"A StringHierarchyLocation can be built from one of " +
            $"the following path specs, selecting one option from any list " +
            $"in [] and excluding the []:\n{baseModelRegion.ToString()}";

        this.ParamLimits = new() {
            {"fullSpec",
                new DynamicDataLimits<string>(this.LocationSpecCheckFunction,
                this.HelpMsg) } };

    }

    private bool LocationSpecCheckFunction(string fullSpec)
    {
        var parts = fullSpec.Split(
            StringHierarchySpec.REGIONS_MODIFIERS_DELIMITER);

        return parts.Length > 0 &&
            // First try to navigate this model to the specified region.
            this._baseRegion.TryGetSubregion(parts[0], out var subregion) &&
            // Then - if any given - check if the modifiers valid in the model.
            (parts.Length > 1) ?
            subregion.IsValidModifierSpec(parts[1]) : true;
    }

    public bool TryCreate(Dictionary<string, object> paramValues,
        out ILocation product, bool skipValueValidation = false)
    {
        bool valid = paramValues.TryGetValue("fullSpec", out object value);
        product = null;
        if (!skipValueValidation)
        {
            valid = this.ParamLimits["fullSpec"].IsValidDataValue(value);
        }
        if (valid)
        {
            product = new StringHierarchyLocation((string)value);
        }
        return valid;
    }
}