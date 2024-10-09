# Caching Proxy

![repo size](https://img.shields.io/github/repo-size/SamRener/caching-proxy?style=for-the-badge)
![language count](https://img.shields.io/github/languages/count/SamRener/caching-proxy?style=for-the-badge)

> Simple proxy server that listen to a port, forward requests to the actual server and cache the responses.

## ☕ Using the Caching Proxy

To use, execute the application on command line passing the port and origin server as args:

```
caching-proxy --port 3000 --origin http://dummyjson.com
```

Possible Args:
```
--port [DESIRED_PORT]: Port that proxy server will be served 
```
```
--origin [SERVER]: Server that will receive the forwarded requests
```
```
--memory-cache: Should use a Memory Cache that will be cleaned when the process finishes?
```
```
--clear-cache: Clear the current cache?
```