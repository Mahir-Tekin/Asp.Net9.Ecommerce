import type { NextConfig } from "next";
import './src/lib/dev-certs';

const nextConfig: NextConfig = {
  basePath: "",
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: "https://localhost:5001/api/:path*" // .NET API URL
      }
    ];
  },
  webpack: (config, { isServer }) => {
    if (isServer) {
      config.experiments = {
        ...config.experiments,
        topLevelAwait: true,
      };
    }
    return config;
  },
};

// Configure Node.js to accept self-signed certificates in development
if (process.env.NODE_ENV !== 'production') {
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
}

module.exports = {
  images: {
    domains: ['localhost'],
  },
};

export default nextConfig;
