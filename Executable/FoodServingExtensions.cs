namespace SystemOfEquations;

internal static class FoodServingExtensions
{
    public static IEnumerable<FoodServing> CombineLikeServings(this IEnumerable<FoodServing> servings) => servings
        .GroupBy(s => s.Name)
        .Select(foodGrouping =>
        {
            // Sum all the nutritional information for foods with the same name
            var first = foodGrouping.First();
            var servingsList = foodGrouping.ToList();
            
            // Calculate total servings and nutrition
            var totalServingUnits = servingsList.Sum(s => s.NutritionalInformation.ServingUnits);
            var totalCals = servingsList.Sum(s => s.NutritionalInformation.Cals);
            var totalP = servingsList.Sum(s => s.NutritionalInformation.P);
            var totalF = servingsList.Sum(s => s.NutritionalInformation.F);
            var totalCTotal = servingsList.Sum(s => s.NutritionalInformation.CTotal);
            var totalCFiber = servingsList.Sum(s => s.NutritionalInformation.CFiber);
            
            var totalNutrition = new NutritionalInformation(
                totalServingUnits,
                first.NutritionalInformation.ServingUnit,
                totalCals,
                totalP,
                totalF,
                totalCTotal,
                totalCFiber);
            
            return new FoodServing(
                first.Name,
                totalNutrition,
                first.Water,
                first.IsConversion);
        });
}