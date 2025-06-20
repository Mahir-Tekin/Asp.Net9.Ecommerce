// Configure Node.js to accept self-signed certificates in development
if (process.env.NODE_ENV !== 'production') {
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
}
 
export {}; 