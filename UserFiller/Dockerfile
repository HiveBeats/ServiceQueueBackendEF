FROM golang:alpine as builder
WORKDIR /app 
COPY ["main.go", "./"]
COPY ["go.mod", "./"]
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o publish/UserFiller -ldflags="-w -s" .

FROM alpine:latest
WORKDIR /app
COPY wait-for.sh ./
COPY --from=builder /app/publish/UserFiller /usr/bin/
ENTRYPOINT ["UserFiller"]
