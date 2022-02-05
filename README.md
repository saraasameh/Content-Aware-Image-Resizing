# Content-Aware-Image-Resizing
Given an image in form of 2D array of values representing its pixels, we need to reduce its size by removing some pixels. Each pixel has a value that indicates its importance based on its color called ENERGY. To avoid disturbing visual effects, we require to obtain the vertical seam with minimum total energy. 

A vertical seam is a vertical connected path of pixels, one pixel in each row. We call two pixels connected if they are vertically (in the same column) or diagonally adjacent.

the task is to calculate the best vertical seam with the min total energy (min distortion) to be removed from the original image.
