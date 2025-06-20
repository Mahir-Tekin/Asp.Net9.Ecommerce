import React, { useRef, useState } from 'react';
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
        // If backend returns a relative path, convert to full URL
        const fullUrl = url.startsWith('http') ? url : `${API_URL}${url}`;
        setImages([
          ...images,
          { url: fullUrl, altText: '', isMain: images.length === 0 },
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
        {images.map((img, idx) => (
          <div key={img.url} className="border rounded p-2 flex flex-col items-center w-40">
            <img src={img.url} alt={img.altText || 'Product image'} className="w-32 h-32 object-cover mb-2" />
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
        ))}
      </div>
    </div>
  );
};

export default ProductImagesManager;
