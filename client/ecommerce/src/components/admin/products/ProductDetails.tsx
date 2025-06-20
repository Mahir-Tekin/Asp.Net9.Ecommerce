// Placeholder for ProductDetails
'use client';

export default function ProductDetails({ id, onEdit, onDelete }: { id: string; onEdit: () => void; onDelete: () => void }) {
  return (
    <div>
      <h3>Product Details (placeholder)</h3>
      <div>Product ID: {id}</div>
      <button onClick={onEdit}>Edit</button>
      <button onClick={onDelete}>Delete</button>
    </div>
  );
}
