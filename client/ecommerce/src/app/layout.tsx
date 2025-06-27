import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { ProductFilterProvider } from "@/context/ProductFilterContext";
import { CartProvider } from "@/context/CartContext";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "E-commerce App",
  description: "Next.js E-commerce Application",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <AuthProvider>
          <ProductFilterProvider>
            <CartProvider>
              {children}
            </CartProvider>
          </ProductFilterProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
