import React, { useRef, useState } from 'react';
import Image from 'next/image';
import { ProductImage } from './types';
import axios from 'axios';

interface ProductImagesManagerProps {
  images: ProductImage[];
  setImages: (images: ProductImage[]) => void;
}

const ProductImagesManager: React.FC<ProductImagesManagerProps> = ({ images, setImages }) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files || files.length === 0) return;
    setUploading(true);
    setError(null);
    const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
    const token = localStorage.getItem('accessToken');
    try {
      for (let i = 0; i < files.length; i++) {
        const formData = new FormData();
        formData.append('file', files[i]);
        const response = await axios.post(`${API_URL}/api/Products/images/upload`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
            Authorization: `Bearer ${token}`,
          },
        });
        
        let url = response.data;
        // Always store only the relative path (without backend URL)
        let relativeUrl = url;
        
        if (url.startsWith('http')) {
          // Remove API_URL prefix if present
          if (url.startsWith(API_URL)) {
            relativeUrl = url.substring(API_URL.length);
          } else {
            // Extract pathname from full URL
            try {
              const urlObj = new URL(url);
              relativeUrl = urlObj.pathname;
            } catch {
              // If URL parsing fails, use the original URL
              relativeUrl = url;
            }
          }
        }
        
        // Ensure the relative URL starts with /
        if (!relativeUrl.startsWith('/') && !relativeUrl.startsWith('http')) {
          relativeUrl = '/' + relativeUrl;
        }
        setImages([
          ...images,
          { url: relativeUrl, altText: '', isMain: images.length === 0 },
        ]);
      }
    } catch (err: any) {
      // Try to extract detailed error message from backend
      let message = 'Image upload failed.';
      if (err.response && err.response.data) {
        if (typeof err.response.data === 'string') {
          message = err.response.data;
        } else if (err.response.data.errors && Array.isArray(err.response.data.errors) && err.response.data.errors.length > 0) {
          message = err.response.data.errors.map((e: any) => e.errorMessage || e.message).join(' ');
        } else if (err.response.data.message) {
          message = err.response.data.message;
        }
      }
      setError(message);
    } finally {
      setUploading(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleAltTextChange = (idx: number, altText: string) => {
    setImages(images.map((img, i) => i === idx ? { ...img, altText } : img));
  };

  const handleSetMain = (idx: number) => {
    setImages(images.map((img, i) => ({ ...img, isMain: i === idx })));
  };

  const handleRemove = (idx: number) => {
    const newImages = images.filter((_, i) => i !== idx);
    // If main image is removed, set first as main
    if (!newImages.some(img => img.isMain) && newImages.length > 0) {
      newImages[0].isMain = true;
    }
    setImages(newImages);
  };

  return (
    <div>
      <label className="block text-sm font-medium mb-1">Product Images</label>
      <input
        type="file"
        accept="image/*"
        multiple
        ref={fileInputRef}
        onChange={handleFileChange}
        disabled={uploading}
        className="mb-2"
      />
      {uploading && <div className="text-blue-500">Uploading...</div>}
      {error && <div className="text-red-500">{error}</div>}
      <div className="flex flex-wrap gap-4 mt-2">
        {images.map((img, idx) => {
          // Handle image URL construction properly
          const getImageSrc = (url: string) => {
            // If URL is already absolute (starts with http/https), use as is
            if (url.startsWith('http://') || url.startsWith('https://')) {
              return url;
            }
            
            // If URL starts with /, it's an absolute path from server root
            if (url.startsWith('/')) {
              const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
              return `${API_URL}${url}`;
            }
            
            // If URL is relative, prepend the API URL and /api path
            const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
            return `${API_URL}/${url.startsWith('/') ? url.slice(1) : url}`;
          };
          
          const src = getImageSrc(img.url);
          
          return (
            <div key={`${img.url}-${idx}`} className="border rounded p-2 flex flex-col items-center w-40">
              <div className="w-32 h-32 mb-2 relative bg-gray-100 rounded overflow-hidden">
                <Image 
                  src={src} 
                  alt={img.altText || 'Product image'} 
                  fill
                  className="object-cover"
                  sizes="128px"
                  onError={(e) => {
                    console.error('Image failed to load:', src);
                    // Show a simple gray placeholder instead
                    const target = e.currentTarget;
                    target.style.display = 'none';
                    const placeholder = target.parentElement?.querySelector('.placeholder');
                    if (placeholder) {
                      (placeholder as HTMLElement).style.display = 'flex';
                    }
                  }}
                />
                <div 
                  className="placeholder absolute inset-0 bg-gray-200 flex items-center justify-center text-gray-500 text-xs" 
                  style={{ display: 'none' }}
                >
                  Image Error
                </div>
              </div>
              <input
                type="text"
                placeholder="Alt text"
                value={img.altText}
                onChange={e => handleAltTextChange(idx, e.target.value)}
                className="w-full border rounded p-1 text-xs mb-1"
                required
              />
              <div className="flex items-center gap-2 mb-1">
                <input
                  type="radio"
                  checked={img.isMain}
                  onChange={() => handleSetMain(idx)}
                  name="mainImage"
                  id={`main-image-${idx}`}
                />
                <label htmlFor={`main-image-${idx}`} className="text-xs">Main</label>
              </div>
              <button
                type="button"
                onClick={() => handleRemove(idx)}
                className="text-xs text-red-600 hover:underline"
              >
                Remove
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default ProductImagesManager;
