
import Navbar from "@/components/layout/Navbar";
import Footer from "@/components/layout/Footer";
import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";

export default function About() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      
      <main className="flex-1 px-6 py-24">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-16">
            <h1 className="text-4xl font-bold tracking-tight sm:text-5xl mb-4">About Execudata</h1>
            <p className="text-xl text-muted-foreground max-w-3xl mx-auto">
              Empowering organizations with data-driven insights and innovative survey solutions
            </p>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-16 mb-20">
            <div>
              <h2 className="text-2xl font-semibold mb-4">Our Mission</h2>
              <p className="text-muted-foreground mb-6">
                At Execudata, our mission is to transform the way organizations collect, analyze, and leverage feedback. 
                We believe that understanding customer and stakeholder opinions is crucial for sustainable growth and 
                continuous improvement.
              </p>
              <p className="text-muted-foreground mb-6">
                Founded in 2010, Execudata has grown from a small startup to a leading provider of survey and feedback 
                solutions, serving clients across industries and around the globe.
              </p>
            </div>
            
            <div>
              <h2 className="text-2xl font-semibold mb-4">Our Approach</h2>
              <p className="text-muted-foreground mb-6">
                We combine cutting-edge technology with deep expertise in data analysis and user experience design to 
                create solutions that are both powerful and intuitive.
              </p>
              <p className="text-muted-foreground mb-6">
                Our team of experts works closely with each client to understand their unique needs and challenges, 
                ensuring that our solutions deliver real, measurable value.
              </p>
            </div>
          </div>
          
          <div className="mb-20">
            <h2 className="text-2xl font-semibold text-center mb-8">Our Values</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
              <div className="bg-muted p-6 rounded-lg">
                <h3 className="text-xl font-medium mb-2">Innovation</h3>
                <p className="text-muted-foreground">
                  We constantly push the boundaries of what's possible, investing in research and development to create 
                  solutions that address emerging challenges.
                </p>
              </div>
              
              <div className="bg-muted p-6 rounded-lg">
                <h3 className="text-xl font-medium mb-2">Integrity</h3>
                <p className="text-muted-foreground">
                  We maintain the highest standards of honesty, transparency, and ethical conduct in all our interactions 
                  with clients, partners, and each other.
                </p>
              </div>
              
              <div className="bg-muted p-6 rounded-lg">
                <h3 className="text-xl font-medium mb-2">Impact</h3>
                <p className="text-muted-foreground">
                  We measure our success by the positive difference we make for our clients and their stakeholders, 
                  focusing on outcomes rather than outputs.
                </p>
              </div>
            </div>
          </div>
          
          <div className="text-center mb-16">
            <h2 className="text-2xl font-semibold mb-6">Ready to Share Your Feedback?</h2>
            <p className="text-lg text-muted-foreground mb-8 max-w-2xl mx-auto">
              Your suggestions and requirements are valuable to us. Access our client portal to share your ideas and help us improve.
            </p>
            <Link to="/client">
              <Button size="lg" className="px-8">
                Access Client Portal
              </Button>
            </Link>
          </div>
          
          <div className="border-t pt-16">
            <h2 className="text-2xl font-semibold text-center mb-8">Contact Us</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
              <div>
                <h3 className="text-lg font-medium mb-1">Email</h3>
                <p className="text-muted-foreground">contact@execudata.com</p>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-1">Phone</h3>
                <p className="text-muted-foreground">+1 (555) 123-4567</p>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-1">Location</h3>
                <p className="text-muted-foreground">101 Tech Plaza, San Francisco, CA</p>
              </div>
            </div>
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
