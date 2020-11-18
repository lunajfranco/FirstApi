using CityInfo.API.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controller
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPointOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(cities => cities.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name ="GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(cities => cities.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            //find point of interest
            var pointOfInterest = city.PointsOfInterest
                .FirstOrDefault(cities => cities.Id == id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);
                
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, 
            [FromBody] PointOfInterestDtoCreate pointsOfInterest)
        {
            if (pointsOfInterest.Description == pointsOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name");
            }

            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(cities => cities.Id == cityId);
            if  (city == null)
            {
                return NotFound();
            }

            //demo purposes - to be improved

            var maxPointsofinteresId = CitiesDataStore.Current.Cities.SelectMany(cities => cities.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInteres = new PointOfInterestDto()
            {
                Id = ++maxPointsofinteresId,
                Name = pointsOfInterest.Name,
                Description = pointsOfInterest.Description
            };
            city.PointsOfInterest.Add(finalPointOfInteres);

            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = finalPointOfInteres.Id },
                finalPointOfInteres);       
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfinterestDtoUpdate pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(cities => cities.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(points => points.Id == id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfinterestDtoUpdate> patchDoc)
        {
            var city = CitiesDataStore.Current.Cities.
                FirstOrDefault(cities => cities.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(points => points.Id == id);
            if (pointOfInterestFromStore == null)
            { 
                return NotFound();
            }

            var pointOfInterestPatch =
                new PointOfinterestDtoUpdate()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };
            patchDoc.ApplyTo(pointOfInterestPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (pointOfInterestPatch.Description == pointOfInterestPatch.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "Should be different from the name ");
            }
            if (!TryValidateModel(pointOfInterestPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestPatch.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.
                FirstOrDefault(cities => cities.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(points => points.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            return NoContent();
        }

    }
}
