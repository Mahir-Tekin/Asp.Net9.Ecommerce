import { useState } from 'react';

interface ProductImageGalleryProps {
  images: { url: string; altText: string; isMain: boolean }[];
}

export default function ProductImageGallery({ images }: ProductImageGalleryProps) {
  const mainImage = images.find(img => img.isMain) || images[0];
  const [selected, setSelected] = useState(mainImage);

  // Always display using full URL if not absolute
  const API_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001';
  const getSrc = (url: string) => url.startsWith('http') ? url : `${API_URL}${url}`;

  return (
    <div className="flex flex-col items-center">
      <div className="w-80 h-80 bg-gray-100 flex items-center justify-center mb-4 border rounded overflow-hidden">
        {selected ? (
          <img src={getSrc(selected.url)} alt={selected.altText} className="object-contain w-full h-full" />
        ) : (
          <span className="text-gray-400">No Image</span>
        )}
      </div>
      {images.length > 1 && (
        <div className="flex gap-2 mt-2">
          {images.map((img, idx) => (
            <button
              key={img.url + idx}
              className={`w-16 h-16 border rounded overflow-hidden focus:outline-none ${selected === img ? 'ring-2 ring-indigo-500' : ''}`}
              onClick={() => setSelected(img)}
              type="button"
            >
              <img src={getSrc(img.url)} alt={img.altText} className="object-contain w-full h-full" />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
