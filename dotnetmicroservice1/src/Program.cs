using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/intersection", ([FromBody] IntersectionRequest request) =>
{
    // best way to get the intersection of two arrays
    var intersection1 = request.Array1.Intersect(request.Array2).ToArray();

    // deconstruct Intersect method to show how it works
    var set1 = new HashSet<int>(request.Array1); // creatinon of HashSet is O(n)
    var intersection2 = new HashSet<int>();
    foreach (var item in request.Array2)
    {
        if (set1.Contains(item)) // O(1) lookup using HashSet totally O(m) where m is the length of the second array
        {
            intersection2.Add(item); // total O(n+m)
        }
    }

    // crude way to get the intersection of two arrays
    var intersection3 = request.Array1.Where(x => request.Array2.Contains(x)).ToArray();

    // expanding where loop using for loops to get the intersection of two arrays
    var intersection4 = new List<int>();
    
    for (int i = 0; i < request.Array1.Length; i++)
    {
        for (int j = 0; j < request.Array2.Length; j++)
        {
            if (request.Array1[i] == request.Array2[j])
            {
                // Check if the element is not already in the intersection                
                bool found = false;
                for (int k = 0; k < intersection4.Count; k++)
                {
                    if (intersection4[k] == request.Array1[i])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    intersection4.Add(request.Array1[i]);
                }
                break; // No need to continue inner loop once a match is found
            }
        }
    }

    var response = new IntersectionResponse
    {
        Input = request,
        Intersections = new Dictionary<IntersectionMethod, int[]>(){
            { IntersectionMethod.IntersectUsingIntersetMethod, intersection1 },
            { IntersectionMethod.HashSetMethod, intersection2.ToArray() },
            { IntersectionMethod.LinqMethod, intersection3 },
            { IntersectionMethod.CrudeForLoopMethod, intersection4.ToArray() }
        },
        Message = "Intersection generated successfully using different methodology"
    };

    return Results.Ok(response);
})
.WithName("GetIntersection")
.WithOpenApi();


app.Run();

public class IntersectionRequest
{
    public int[] Array1 { get; set; } = Array.Empty<int>();
    public int[] Array2 { get; set; } = Array.Empty<int>();
}

public class IntersectionResponse
{
    public IntersectionRequest Input { get; set; } = new IntersectionRequest();
    public IDictionary<IntersectionMethod, int[]> Intersections { get; set; } = new Dictionary<IntersectionMethod, int[]>();
    public string Message { get; set; } = string.Empty;
}

public enum IntersectionMethod
{
    IntersectUsingIntersetMethod,
    HashSetMethod,
    LinqMethod,
    CrudeForLoopMethod
}