# Use the official Golang image as the base image
FROM golang:1.17-alpine AS build

# Set the working directory
WORKDIR /app

# Copy go mod and sum files
COPY go.mod go.sum ./

# Download all dependencies
RUN go mod download

# Copy the source code
COPY . .

# Build the application
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o main .

# Use a smaller base image for the final image
FROM alpine:latest  

# Set the working directory
WORKDIR /root/

# Copy the binary from the build stage
COPY --from=build /app/main .

# Expose port 8081
EXPOSE 8081

# Set environment variable for the port
ENV PORT=8081

# Run the binary
CMD ["./main"]