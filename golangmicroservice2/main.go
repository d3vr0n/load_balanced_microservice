package main

import (
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
)

type InputArrays struct {
	Array1 []int `json:"array1"`
	Array2 []int `json:"array2"`
}

type Response struct {
	Input         InputArrays `json:"input"`
	Intersections []int       `json:"intersections"`
	Message       string      `json:"message"`
}

func main() {
	r := gin.Default()

	port := os.Getenv("PORT")
	if port == "" {
		port = "8081"
	}

	r.POST("/intersection", getIntersection)
	r.Run(":" + port)
}

func getIntersection(c *gin.Context) {
	var input InputArrays

	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	intersection := findIntersection(input.Array1, input.Array2)
	response := Response{Input: input, Intersections: intersection, Message: "Intersection of two arrays in golang"}

	c.JSON(http.StatusOK, response)
}

func findIntersection(arr1, arr2 []int) []int {
	set := make(map[int]bool)
	var intersection []int

	for _, num := range arr1 {
		set[num] = true
	}

	for _, num := range arr2 {
		if set[num] {
			intersection = append(intersection, num)
			delete(set, num) // To avoid duplicates
		}
	}

	return intersection
}
