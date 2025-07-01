interface ProductDescriptionProps {
  description: string;
}

export default function ProductDescription({ description }: ProductDescriptionProps) {
  if (!description) {
    return (
      <div className="text-gray-500 italic">
        No description available for this product.
      </div>
    );
  }
  
  return (
    <div className="text-gray-700 leading-relaxed">
      <div className="whitespace-pre-wrap">{description}</div>
    </div>
  );
}
