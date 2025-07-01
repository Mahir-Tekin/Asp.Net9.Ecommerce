'use client';

import Header from "@/components/layout/Header/Header";
import Footer from "@/components/layout/Footer/Footer";

export default function FavoritesLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <div className="min-h-screen">
      <Header />
      <div className="pt-[70px]">
        <main>
          {children}
        </main>
      </div>
      <Footer />
    </div>
  );
}
