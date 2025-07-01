import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { CartProvider } from "@/context/CartContext";
import { ToastProvider } from "@/context/ToastContext";
import ToastContainer from "@/components/ui/ToastContainer";

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
      <body className={`${inter.className} bg-gray-50 min-h-screen`}>
        <div className="w-full max-w-7xl mx-auto min-h-screen">
          <AuthProvider>
              <CartProvider>
                <ToastProvider>
                  {children}
                  <ToastContainer />
                </ToastProvider>
              </CartProvider>
          </AuthProvider>
        </div>
      </body>
    </html>
  );
}
