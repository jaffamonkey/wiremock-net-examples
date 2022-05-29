# Run WiremockNET as service

WiremockNet uses the `__admin` folder to read json mappings and input files from.

```bash
dotnet dev-certs https --trust
dotnet-wiremock --root-dir /home/pablo/code/wiremock-net-examples/WireMockNetExamples --ReadStaticMappings true
```

## (For Linux)Make the localhost.conf file of content

```
[req]
default_bits       = 2048
default_keyfile    = localhost.key
distinguished_name = req_distinguished_name
req_extensions     = req_ext
x509_extensions    = v3_ca

[req_distinguished_name]
commonName         = Common Name (e.g. server FQDN or YOUR name)

[req_ext]
subjectAltName = @alt_names

[v3_ca]
subjectAltName = @alt_names
basicConstraints = critical, CA:false
keyUsage = keyCertSign, cRLSign, digitalSignature,keyEncipherment
extendedKeyUsage = 1.3.6.1.5.5.7.3.1
1.3.6.1.4.1.311.84.1.1 = DER:01

[alt_names]
DNS.1   = localhost
DNS.2   = 127.0.0.1
```

## Generate the cert

```
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -subj /CN=localhost
openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -passout pass:
```

Grab the localhost.pfx and localhost.crt and copy these files into the target system. 

In case of Docker that would look:

```
COPY localhost.crt /usr/local/share/ca-certificates/
RUN dotnet dev-certs https --clean \
    && update-ca-certificates
COPY localhost.pfx /root/.dotnet/corefx/cryptography/x509stores/my/
```

# WiremockNET Command line options

--port: Set the HTTP port number e.g. --port 9999. Use --port 0 to dynamically determine a port.

--disable-http: Disable the HTTP listener, option available only if HTTPS is enabled.

--https-port: If specified, enables HTTPS on the supplied port. Note: When you specify this parameter, WireMock will still, additionally, bind to an HTTP port (8080 by default). So when running multiple WireMock servers you will also need to specify the --port parameter in order to avoid conflicts.

--bind-address: The IP address the WireMock server should serve from. Binds to all local network adapters if unspecified.

--https-keystore: Path to a keystore file containing an SSL certificate to use with HTTPS. Can be a path to a file or a resource on the classpath. The keystore must have a password of “password”. This option will only work if --https-port is specified. If this option isn’t used WireMock will default to its own self-signed certificate.

--keystore-type: The HTTPS keystore type. Usually JKS or PKCS12.

--keystore-password: Password to the keystore, if something other than “password”. Note: the behaviour of this changed in version 2.27.0. Previously this set Jetty’s key manager password, whereas now it sets the keystore password value. The key manager password can be set with the (new) parameter below.

--key-manager-password: The password used by Jetty to access individual keys in the store, if something other than “password”.

--https-truststore: Path to a keystore file containing client public certificates, proxy target public certificates & private keys to use when authenticate with a proxy target that require client authentication. Can be a path to a file or a resource on the classpath. See HTTPS configuration and Running as a browser proxy for details.

--truststore-type: The HTTPS trust store type. Usually JKS or PKCS12.

--truststore-password: Optional password to the trust store. Defaults to “password” if not specified.

--https-require-client-cert: Force clients to authenticate with a client certificate. See https for details.

--verbose: Turn on verbose logging to stdout

--root-dir: Sets the root directory, under which mappings and __files reside. This defaults to the current directory.

--record-mappings: Record incoming requests as stub mappings. See record-playback.

--match-headers: When in record mode, capture request headers with the keys specified. See record-playback.

--proxy-all: Proxy all requests through to another base URL e.g. --proxy-all="http://api.someservice.com" Typically used in conjunction with --record-mappings such that a session on another service can be recorded.

--preserve-host-header: When in proxy mode, it passes the Host header as it comes from the client through to the proxied service. When this option is not present, the Host header value is deducted from the proxy URL. This option is only available if the --proxy-all option is specified.

--proxy-via: When proxying requests (either by using –proxy-all or by creating stub mappings that proxy to other hosts), route via another proxy server (useful when inside a corporate network that only permits internet access via an opaque proxy). e.g. --proxy-via webproxy.mycorp.com (defaults to port 80) or --proxy-via webproxy.mycorp.com:8080. Also supports proxy authentication, e.g. --proxy-via http://username:password@webproxy.mycorp.com:8080/.

--enable-browser-proxying: Run as a browser proxy. See browser-proxying.

--ca-keystore: A key store containing a root Certificate Authority private key and certificate that can be used to sign generated certificates when browser proxying https. Defaults to $HOME/.wiremock/ca-keystore.jks.

--ca-keystore-password: Password to the ca-keystore, if something other than “password”.

--ca-keystore-type: Type of the ca-keystore, if something other than jks.

--trust-all-proxy-targets: Trust all remote certificates when running as a browser proxy and proxying HTTPS traffic.

--trust-proxy-target: Trust a specific remote endpoint’s certificate when running as a browser proxy and proxying HTTPS traffic. Can be specified multiple times. e.g. --trust-proxy-target dev.mycorp.com --trust-proxy-target localhost would allow proxying to https://dev.mycorp.com or http://localhost:9876 despite their having invalid certificate chains in some way.

--no-request-journal: Disable the request journal, which records incoming requests for later verification. This allows WireMock to be run (and serve stubs) for long periods (without resetting) without exhausting the heap. The --record-mappings option isn’t available if this one is specified.

--container-threads: The number of threads created for incoming requests. Defaults to 10.

--max-request-journal-entries: Set maximum number of entries in request journal (if enabled). When this limit is reached oldest entries will be discarded.

--jetty-acceptor-threads: The number of threads Jetty uses for accepting requests.

--jetty-accept-queue-size: The Jetty queue size for accepted requests.

--jetty-header-buffer-size: Deprecated, use --jetty-header-request-size. The Jetty buffer size for request headers, e.g. --jetty-header-buffer-size 16384, defaults to 8192K.

--jetty-header-request-size: The Jetty buffer size for request headers, e.g. --jetty-header-request-size 16384, defaults to 8192K.

--jetty-header-response-size: The Jetty buffer size for response headers, e.g. --jetty-header-response-size 16384, defaults to 8192K.

--async-response-enabled: Enable asynchronous request processing in Jetty. Recommended when using WireMock for performance testing with delays, as it allows much more efficient use of container threads and therefore higher throughput. Defaults to false.

--async-response-threads: Set the number of asynchronous (background) response threads. Effective only with asynchronousResponseEnabled=true. Defaults to 10.

--extensions: Extension class names e.g. com.mycorp.HeaderTransformer,com.mycorp.BodyTransformer. See extending-wiremock.

--print-all-network-traffic: Print all raw incoming and outgoing network traffic to console.

--global-response-templating: Render all response definitions using Handlebars templates.

--local-response-templating: Enable rendering of response definitions using Handlebars templates for specific stub mappings.

--max-template-cache-entries: Set the maximum number of compiled template fragments to cache. Only has any effect when response templating is enabled. Defaults to no limit.

--use-chunked-encoding: Set the policy for sending responses with Transfer-Encoding: chunked. Valid values are always, never and body_file. The last of these will cause chunked encoding to be used only when a stub defines its response body from a file.

--disable-gzip: Prevent response bodies from being gzipped.

--disable-request-logging: Prevent requests and responses from being sent to the notifier. Use this when performance testing as it will save memory and CPU even when info/verbose logging is not enabled.

--disable-banner: Prevent WireMock logo from being printed on startup

--permitted-system-keys: Comma-separated list of regular expressions for names of permitted environment variables and system properties accessible from response templates. Only has any effect when templating is enabled. Defaults to wiremock.*.

--enable-stub-cors: Enable automatic sending of cross-origin (CORS) response headers. Defaults to off.

--help: Show command line help
Configuring WireMock using the Java client
