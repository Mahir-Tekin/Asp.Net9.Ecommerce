import Header from '@/components/layout/Header/Header';
import Footer from '@/components/layout/Footer/Footer';

export default function ProductLayout({ children }: { children: React.ReactNode }) {
  return (
    <>
      <Header />
      <main className="min-h-screen pt-16 pb-24 bg-gray-50">
        {children}
      </main>
      <Footer />
    </>
  );
}
