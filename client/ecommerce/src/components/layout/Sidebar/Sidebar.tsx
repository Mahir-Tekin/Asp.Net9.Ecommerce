'use client';

import { useState } from 'react';

export default function Sidebar() {
  const [isOpen, setIsOpen] = useState(true);

  return (
    <div className="w-64 bg-green-500 h-[calc(100vh-4rem)] p-4 flex flex-col items-center justify-center text-white text-lg font-bold">
      Sidebar
    </div>
  );
} 