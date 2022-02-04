FROM golang:latest as builder
WORKDIR /go/src
COPY fullCylcleRocks.go /go/src
RUN go build fullCylcleRocks.go

FROM scratch
WORKDIR /go/src
COPY --from=builder /go/src .
CMD [ "./fullCylcleRocks" ]
