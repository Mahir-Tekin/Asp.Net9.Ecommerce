// Portfolio-focused homepage showcasing technical capabilities
import { useEffect, useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { HiShoppingBag, HiHeart, HiStar } from 'react-icons/hi2';
import { createFilterURL } from '@/utils/urlFilters';

interface Product {
  id: string;
  slug: string;
  name: string;
  lowestPrice: number;
  lowestOldPrice?: number;
  mainImage?: string;
  averageRating: number;
  reviewCount: number;
  hasStock: boolean;
  isActive: boolean;
}

export default function HomePageContent() {
  const [featuredProducts, setFeaturedProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchFeaturedProducts() {
      try {
        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001'}/api/Products?pageSize=8&pageNumber=1`);
        if (res.ok) {
          const data = await res.json();
          setFeaturedProducts(data.items || data || []);
        }
      } catch (error) {
        console.error('Failed to fetch featured products:', error);
      } finally {
        setLoading(false);
      }
    }
    
    fetchFeaturedProducts();
  }, []);

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  return (
    <div className="min-h-screen">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Hero Section - Portfolio Project Showcase */}
        <section className="mb-20">
          <div className="bg-gradient-to-br from-blue-600 via-purple-600 to-blue-800 rounded-2xl overflow-hidden shadow-2xl">
            <div className="px-8 md:px-16 py-20 text-white">
              <div className="max-w-4xl mx-auto text-center">
                <div className="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm px-4 py-2 rounded-full text-sm font-medium mb-6">
                  <span className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></span>
                  Portfolio Project
                </div>
                <h1 className="text-5xl md:text-6xl font-bold mb-6 leading-tight">
                  Full-Stack <span className="text-blue-200">E-Commerce</span> Platform
                </h1>
                <p className="text-xl md:text-2xl mb-8 opacity-90 max-w-3xl mx-auto">
                  An enterprise-grade e-commerce platform showcasing advanced backend architecture with Clean Architecture, CQRS, Domain-Driven Design, and modern development practices
                </p>
                
                {/* Tech Stack Pills */}
                <div className="flex flex-wrap justify-center gap-3 mb-8">
                  {['ASP.NET 9', 'Clean Architecture', 'CQRS + MediatR', 'Domain-Driven Design', 'Entity Framework', 'JWT + Refresh Tokens', 'PostgreSQL', 'GitHub Actions', 'Vercel Deployment', 'Next.js 14', 'TypeScript'].map((tech) => (
                    <span key={tech} className="px-4 py-2 bg-white/20 backdrop-blur-sm rounded-full text-sm font-medium">
                      {tech}
                    </span>
                  ))}
                </div>

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row gap-4 justify-center">
                  <a 
                    href="https://github.com/yourusername/project-repo" 
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-2 bg-white text-blue-600 px-8 py-4 rounded-xl font-bold text-lg hover:bg-blue-50 hover:scale-105 transition-all duration-300 shadow-lg"
                  >
                    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
                    </svg>
                    View Source Code
                  </a>
                  <a 
                    href={createFilterURL({ categoryId: 'all' })}
                    className="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm text-white px-8 py-4 rounded-xl font-bold text-lg hover:bg-white/20 hover:scale-105 transition-all duration-300 border border-white/20"
                  >
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                    </svg>
                    Explore Demo
                  </a>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Key Features Section */}
        <section className="mb-20">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold mb-4 text-gray-800">Enterprise-Grade Backend Architecture</h2>
            <p className="text-gray-600 text-lg">Advanced software engineering patterns and modern development practices</p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: "🏗️",
                title: "Clean Architecture",
                description: "Domain-driven design with clear separation of concerns. Domain, Application, Infrastructure layers following SOLID principles",
                color: "bg-blue-100"
              },
              {
                icon: "⚡",
                title: "CQRS + MediatR",
                description: "Command Query Responsibility Segregation with MediatR for scalable request/response handling and clean controller design",
                color: "bg-green-100"
              },
              {
                icon: "�",
                title: "Advanced Authentication",
                description: "JWT access tokens with secure refresh token rotation, role-based authorization, and ASP.NET Identity integration",
                color: "bg-purple-100"
              },
              {
                icon: "🛡️",
                title: "Result Pattern",
                description: "Type-safe error handling without exceptions. Explicit success/failure states with detailed error responses",
                color: "bg-red-100"
              },
              {
                icon: "🏪",
                title: "Repository + UoW",
                description: "Repository pattern with Unit of Work for clean data access abstraction and transaction management",
                color: "bg-yellow-100"
              },
              {
                icon: "�",
                title: "Rich Domain Models",
                description: "Domain entities with encapsulated business rules, value objects, and aggregate boundaries for data consistency",
                color: "bg-indigo-100"
              }
            ].map((feature, index) => (
              <div key={index} className="p-6 bg-white rounded-xl shadow-lg border border-gray-100 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
                <div className={`w-16 h-16 ${feature.color} rounded-full flex items-center justify-center mx-auto mb-4`}>
                  <span className="text-2xl">{feature.icon}</span>
                </div>
                <h3 className="text-xl font-bold mb-3 text-gray-800">{feature.title}</h3>
                <p className="text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </section>

        {/* Advanced Backend Features */}
        <section className="mb-20">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold mb-4 text-gray-800">Backend Implementation Highlights</h2>
            <p className="text-gray-600 text-lg">Sophisticated patterns and enterprise-level code organization</p>
          </div>
          
          <div className="grid lg:grid-cols-2 gap-8 mb-12">
            {/* Authentication System */}
            <div className="bg-gradient-to-br from-purple-50 to-purple-100 rounded-xl p-8 border border-purple-200">
              <div className="flex items-center mb-4">
                <div className="w-12 h-12 bg-purple-500 rounded-lg flex items-center justify-center mr-4">
                  <span className="text-white text-xl">🔐</span>
                </div>
                <h3 className="text-2xl font-bold text-gray-800">Advanced Authentication</h3>
              </div>
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-purple-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">JWT + Refresh Tokens:</strong>
                    <p className="text-gray-600">Secure token rotation with configurable expiry times</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-purple-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Role-Based Access:</strong>
                    <p className="text-gray-600">Granular permissions with Admin/Customer roles</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-purple-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">ASP.NET Identity:</strong>
                    <p className="text-gray-600">Built-in security features with custom user model</p>
                  </div>
                </div>
              </div>
            </div>

            {/* CQRS Pattern */}
            <div className="bg-gradient-to-br from-blue-50 to-blue-100 rounded-xl p-8 border border-blue-200">
              <div className="flex items-center mb-4">
                <div className="w-12 h-12 bg-blue-500 rounded-lg flex items-center justify-center mr-4">
                  <span className="text-white text-xl">⚡</span>
                </div>
                <h3 className="text-2xl font-bold text-gray-800">CQRS + MediatR</h3>
              </div>
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-blue-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Command/Query Separation:</strong>
                    <p className="text-gray-600">Clear distinction between read and write operations</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-blue-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Handler Pattern:</strong>
                    <p className="text-gray-600">Dedicated handlers for each business operation</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-blue-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Clean Controllers:</strong>
                    <p className="text-gray-600">Thin controllers that delegate to MediatR</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="grid lg:grid-cols-2 gap-8">
            {/* Domain Models */}
            <div className="bg-gradient-to-br from-green-50 to-green-100 rounded-xl p-8 border border-green-200">
              <div className="flex items-center mb-4">
                <div className="w-12 h-12 bg-green-500 rounded-lg flex items-center justify-center mr-4">
                  <span className="text-white text-xl">🏗️</span>
                </div>
                <h3 className="text-2xl font-bold text-gray-800">Rich Domain Models</h3>
              </div>
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-green-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Encapsulated Business Logic:</strong>
                    <p className="text-gray-600">Domain entities contain their own validation rules</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-green-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Aggregate Boundaries:</strong>
                    <p className="text-gray-600">Proper transactional consistency guarantees</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-green-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Value Objects:</strong>
                    <p className="text-gray-600">Immutable objects for domain concepts</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Error Handling */}
            <div className="bg-gradient-to-br from-red-50 to-red-100 rounded-xl p-8 border border-red-200">
              <div className="flex items-center mb-4">
                <div className="w-12 h-12 bg-red-500 rounded-lg flex items-center justify-center mr-4">
                  <span className="text-white text-xl">🛡️</span>
                </div>
                <h3 className="text-2xl font-bold text-gray-800">Result Pattern</h3>
              </div>
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-red-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">No Exceptions for Business Logic:</strong>
                    <p className="text-gray-600">Explicit success/failure states</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-red-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Type-Safe Error Handling:</strong>
                    <p className="text-gray-600">Compile-time error checking</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="w-2 h-2 bg-red-500 rounded-full mt-2"></span>
                  <div>
                    <strong className="text-gray-800">Structured Error Responses:</strong>
                    <p className="text-gray-600">Consistent API error format</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Technology Stack Section */}
        <section className="mb-20">
          <div className="bg-gradient-to-r from-gray-50 to-blue-50 rounded-2xl p-8 md:p-12">
            <div className="text-center mb-10">
              <h2 className="text-3xl font-bold mb-4 text-gray-800">Technical Implementation Details</h2>
              <p className="text-gray-600 text-lg">Enterprise patterns and industry best practices</p>
            </div>
            
            <div className="grid md:grid-cols-2 gap-8">
              {/* Backend */}
              <div className="bg-white rounded-xl p-6 shadow-md">
                <h3 className="text-xl font-bold mb-4 text-gray-800 flex items-center gap-2">
                  <span className="w-2 h-2 bg-purple-500 rounded-full"></span>
                  Backend Architecture
                </h3>
                <div className="space-y-2">
                  {[
                    'ASP.NET Core 9 Web API',
                    'Clean Architecture (Domain/Application/Infrastructure)',
                    'CQRS pattern with MediatR',
                    'Domain-Driven Design principles',
                    'Entity Framework Core with PostgreSQL',
                    'Repository pattern with Unit of Work',
                    'Result pattern for error handling',
                    'JWT authentication with refresh tokens',
                    'Role-based authorization system',
                    'AutoMapper for object mapping',
                    'FluentValidation for input validation'
                  ].map((tech) => (
                    <div key={tech} className="flex items-center gap-2 text-gray-600">
                      <span className="w-1.5 h-1.5 bg-purple-400 rounded-full"></span>
                      {tech}
                    </div>
                  ))}
                </div>
              </div>

              {/* Frontend */}
              <div className="bg-white rounded-xl p-6 shadow-md">
                <h3 className="text-xl font-bold mb-4 text-gray-800 flex items-center gap-2">
                  <span className="w-2 h-2 bg-blue-500 rounded-full"></span>
                  Frontend & User Experience
                </h3>
                <div className="space-y-2">
                  {[
                    'Next.js 14 with App Router',
                    'TypeScript for type safety',
                    'Tailwind CSS for responsive design',
                    'React Context for state management',
                    'Custom hooks for data fetching',
                    'Protected routes with JWT validation',
                    'Optimistic UI updates',
                    'Client-side caching strategies',
                    'Responsive mobile-first design',
                    'SEO optimization',
                    'Performance optimization'
                  ].map((tech) => (
                    <div key={tech} className="flex items-center gap-2 text-gray-600">
                      <span className="w-1.5 h-1.5 bg-blue-400 rounded-full"></span>
                      {tech}
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Project Stats */}
        <section className="mb-20">
          <div className="bg-white rounded-2xl p-8 md:p-12 shadow-lg border border-gray-100">
            <div className="text-center mb-10">
              <h2 className="text-3xl font-bold mb-4 text-gray-800">Project Complexity & Scale</h2>
              <p className="text-gray-600 text-lg">Demonstrating enterprise-level software architecture</p>
            </div>
            
            <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
              {[
                { number: "50+", label: "Command/Query Handlers", icon: "⚡" },
                { number: "20+", label: "Domain Entities", icon: "🏗️" },
                { number: "15+", label: "API Controllers", icon: "�" },
                { number: "4", label: "Architecture Layers", icon: "📐" }
              ].map((stat, index) => (
                <div key={index} className="text-center">
                  <div className="text-3xl mb-2">{stat.icon}</div>
                  <div className="text-3xl font-bold text-blue-600 mb-1">{stat.number}</div>
                  <div className="text-gray-600 font-medium">{stat.label}</div>
                </div>
              ))}
            </div>

            {/* Architecture Layers Breakdown */}
            <div className="mt-12 grid md:grid-cols-4 gap-4">
              {[
                { layer: "Domain", description: "Business logic & entities", color: "bg-blue-100 text-blue-800" },
                { layer: "Application", description: "Use cases & handlers", color: "bg-green-100 text-green-800" },
                { layer: "Infrastructure", description: "Data access & external services", color: "bg-purple-100 text-purple-800" },
                { layer: "API", description: "Controllers & middleware", color: "bg-orange-100 text-orange-800" }
              ].map((item, index) => (
                <div key={index} className="text-center">
                  <div className={`${item.color} px-3 py-2 rounded-lg font-semibold mb-2`}>
                    {item.layer}
                  </div>
                  <p className="text-sm text-gray-600">{item.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* Call to Action */}
        <section className="text-center bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl py-16 px-8 text-white">
          <h2 className="text-3xl font-bold mb-4">Ready to Explore?</h2>
          <p className="mb-8 text-lg opacity-90">Experience the full e-commerce functionality or dive into the code</p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <a 
              href={createFilterURL({ categoryId: 'all' })}
              className="inline-flex items-center gap-2 bg-white text-blue-600 px-8 py-4 rounded-xl font-bold text-lg hover:bg-blue-50 hover:scale-105 transition-all duration-300 shadow-lg"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
              Start Shopping Demo
            </a>
            <a 
              href="/login" 
              className="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm text-white px-8 py-4 rounded-xl font-bold text-lg hover:bg-white/20 hover:scale-105 transition-all duration-300 border border-white/20"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              Try User Features
            </a>
          </div>
        </section>
      </div>
    </div>
  );
}
