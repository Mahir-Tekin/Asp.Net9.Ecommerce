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
        const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api'}/Products?pageSize=8&pageNumber=1`);
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
      <div className="max-w-7xl mx-auto px-4 py-16">
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
                  A comprehensive e-commerce solution built with modern technologies, showcasing advanced development skills and industry best practices
                </p>
                
                {/* Tech Stack Pills */}
                <div className="flex flex-wrap justify-center gap-3 mb-8">
                  {['Next.js 14', 'ASP.NET Core', 'TypeScript', 'Tailwind CSS', 'JWT Auth', 'PostgreSQL'].map((tech) => (
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
            <h2 className="text-4xl font-bold mb-4 text-gray-800">Technical Highlights</h2>
            <p className="text-gray-600 text-lg">Advanced features demonstrating full-stack expertise</p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {[
              {
                icon: "🔐",
                title: "JWT Authentication",
                description: "Secure user authentication with role-based access control and protected routes",
                color: "bg-blue-100"
              },
              {
                icon: "🛒",
                title: "Advanced Cart System",
                description: "Persistent shopping cart with real-time updates and variant management",
                color: "bg-green-100"
              },
              {
                icon: "💳",
                title: "Order Management",
                description: "Complete order lifecycle with status tracking and detailed history",
                color: "bg-purple-100"
              },
              {
                icon: "❤️",
                title: "Wishlist System",
                description: "Client-side favorites with localStorage persistence and cross-component sync",
                color: "bg-red-100"
              },
              {
                icon: "🔍",
                title: "Smart Filtering",
                description: "Dynamic product filtering with category hierarchy and search functionality",
                color: "bg-yellow-100"
              },
              {
                icon: "📱",
                title: "Responsive Design",
                description: "Mobile-first approach with modern UI components and smooth animations",
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

        {/* Featured Products Showcase */}
        <section className="mb-20">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold mb-4 text-gray-800">Live Product Demo</h2>
            <p className="text-gray-600 text-lg">Real products from the database - try adding to cart and favorites!</p>
          </div>
          
          {loading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {[1, 2, 3, 4].map((i) => (
                <div key={i} className="bg-white rounded-xl shadow-md border border-gray-100 p-4">
                  <div className="aspect-square bg-gray-200 rounded-lg animate-pulse mb-4"></div>
                  <div className="h-4 bg-gray-200 rounded animate-pulse mb-2"></div>
                  <div className="h-6 bg-gray-200 rounded animate-pulse mb-3"></div>
                  <div className="h-10 bg-gray-200 rounded animate-pulse"></div>
                </div>
              ))}
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {featuredProducts.slice(0, 8).map((product) => (
                <div key={product.id} className="bg-white rounded-xl shadow-md border border-gray-100 overflow-hidden hover:shadow-xl transition-all duration-300 hover:-translate-y-1 group">
                  {/* Product Image */}
                  <div className="relative aspect-square bg-gray-100">
                    <Link href={`/product/${product.slug}`}>
                      {product.mainImage ? (
                        <Image
                          src={product.mainImage}
                          alt={product.name}
                          fill
                          className="object-cover group-hover:scale-105 transition-transform duration-300"
                        />
                      ) : (
                        <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-200 to-gray-300">
                          <HiShoppingBag className="w-12 h-12 text-gray-400" />
                        </div>
                      )}
                    </Link>
                    
                    {/* Quick Actions */}
                    <div className="absolute top-3 right-3 opacity-0 group-hover:opacity-100 transition-opacity">
                      <button className="p-2 bg-white/90 backdrop-blur-sm rounded-full shadow-md hover:bg-white transition-colors">
                        <HiHeart className="w-4 h-4 text-gray-600 hover:text-red-500" />
                      </button>
                    </div>

                    {/* Discount Badge */}
                    {product.lowestOldPrice && product.lowestOldPrice > product.lowestPrice && (
                      <div className="absolute top-3 left-3 bg-red-500 text-white px-2 py-1 rounded-full text-xs font-semibold">
                        {Math.round(((product.lowestOldPrice - product.lowestPrice) / product.lowestOldPrice) * 100)}% OFF
                      </div>
                    )}

                    {/* Stock Status */}
                    {!product.isActive || !product.hasStock && (
                      <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                        <span className="bg-white text-gray-900 px-3 py-1 rounded-full text-sm font-medium">
                          {!product.isActive ? 'Unavailable' : 'Out of Stock'}
                        </span>
                      </div>
                    )}
                  </div>

                  {/* Product Info */}
                  <div className="p-4">
                    <Link href={`/product/${product.slug}`}>
                      <h3 className="font-semibold text-gray-900 hover:text-purple-600 transition-colors mb-2 line-clamp-2 min-h-[3rem]">
                        {product.name}
                      </h3>
                    </Link>
                    
                    {/* Rating */}
                    {product.reviewCount > 0 && (
                      <div className="flex items-center gap-1 mb-2">
                        <div className="flex">
                          {[1, 2, 3, 4, 5].map((star) => (
                            <HiStar
                              key={star}
                              className={`w-4 h-4 ${
                                star <= product.averageRating ? 'text-yellow-400' : 'text-gray-300'
                              }`}
                            />
                          ))}
                        </div>
                        <span className="text-sm text-gray-500">({product.reviewCount})</span>
                      </div>
                    )}
                    
                    {/* Price */}
                    <div className="flex items-center gap-2 mb-3">
                      {product.lowestOldPrice && product.lowestOldPrice > product.lowestPrice ? (
                        <>
                          <span className="text-lg font-bold text-gray-900">
                            {formatPrice(product.lowestPrice)}
                          </span>
                          <span className="text-sm text-gray-500 line-through">
                            {formatPrice(product.lowestOldPrice)}
                          </span>
                        </>
                      ) : (
                        <span className="text-lg font-bold text-gray-900">
                          {formatPrice(product.lowestPrice)}
                        </span>
                      )}
                    </div>

                    {/* Action Button */}
                    <Link
                      href={`/product/${product.slug}`}
                      className="w-full bg-gradient-to-r from-purple-600 to-blue-600 text-white py-2 px-4 rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all duration-200 text-center font-medium block text-sm"
                    >
                      View Details
                    </Link>
                  </div>
                </div>
              ))}
            </div>
          )}
          
          {/* View All Products Link */}
          <div className="text-center mt-10">
            <Link
              href={createFilterURL({ categoryId: 'all' })}
              className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white px-8 py-3 rounded-xl font-semibold hover:from-blue-700 hover:to-purple-700 transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              <HiShoppingBag className="w-5 h-5" />
              Browse All Products
            </Link>
          </div>
        </section>

        {/* Technology Stack Section */}
        <section className="mb-20">
          <div className="bg-gradient-to-r from-gray-50 to-blue-50 rounded-2xl p-8 md:p-12">
            <div className="text-center mb-10">
              <h2 className="text-3xl font-bold mb-4 text-gray-800">Technology Stack</h2>
              <p className="text-gray-600 text-lg">Modern tools and frameworks for scalable development</p>
            </div>
            
            <div className="grid md:grid-cols-2 gap-8">
              {/* Frontend */}
              <div className="bg-white rounded-xl p-6 shadow-md">
                <h3 className="text-xl font-bold mb-4 text-gray-800 flex items-center gap-2">
                  <span className="w-2 h-2 bg-blue-500 rounded-full"></span>
                  Frontend
                </h3>
                <div className="space-y-2">
                  {[
                    'Next.js 14 (App Router)',
                    'TypeScript for type safety',
                    'Tailwind CSS for styling',
                    'React Context for state management',
                    'Heroicons for consistent iconography'
                  ].map((tech) => (
                    <div key={tech} className="flex items-center gap-2 text-gray-600">
                      <span className="w-1.5 h-1.5 bg-blue-400 rounded-full"></span>
                      {tech}
                    </div>
                  ))}
                </div>
              </div>

              {/* Backend */}
              <div className="bg-white rounded-xl p-6 shadow-md">
                <h3 className="text-xl font-bold mb-4 text-gray-800 flex items-center gap-2">
                  <span className="w-2 h-2 bg-purple-500 rounded-full"></span>
                  Backend
                </h3>
                <div className="space-y-2">
                  {[
                    'ASP.NET Core Web API',
                    'Entity Framework Core',
                    'PostgreSQL database',
                    'JWT token authentication',
                    'RESTful API architecture'
                  ].map((tech) => (
                    <div key={tech} className="flex items-center gap-2 text-gray-600">
                      <span className="w-1.5 h-1.5 bg-purple-400 rounded-full"></span>
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
              <h2 className="text-3xl font-bold mb-4 text-gray-800">Project Complexity</h2>
              <p className="text-gray-600 text-lg">Demonstrating comprehensive full-stack development skills</p>
            </div>
            
            <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
              {[
                { number: "15+", label: "Components", icon: "🧩" },
                { number: "8+", label: "API Endpoints", icon: "🔗" },
                { number: "5+", label: "User Roles", icon: "👥" },
                { number: "100%", label: "TypeScript", icon: "⚡" }
              ].map((stat, index) => (
                <div key={index} className="text-center">
                  <div className="text-3xl mb-2">{stat.icon}</div>
                  <div className="text-3xl font-bold text-blue-600 mb-1">{stat.number}</div>
                  <div className="text-gray-600 font-medium">{stat.label}</div>
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
