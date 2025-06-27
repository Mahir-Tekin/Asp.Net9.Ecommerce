interface ProductDescriptionProps {
  description: string;
}

export default function ProductDescription({ description }: ProductDescriptionProps) {
  if (!description) return null;
  return (
    <div className="mt-8 text-gray-700">
      <h2 className="text-lg font-semibold mb-2">Description</h2>
      <div>{description}</div>
    </div>
  );
}
