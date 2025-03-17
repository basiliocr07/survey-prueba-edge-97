
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import Navbar from "@/components/layout/Navbar";
import Footer from "@/components/layout/Footer";

export default function About() {
  return (
    <div className="min-h-screen bg-background flex flex-col">
      <Navbar />
      
      <main className="flex-1 container mx-auto pt-24 pb-10 px-4 md:px-6">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold tracking-tight mb-4">About Execudata</h1>
          <p className="text-muted-foreground mb-8">
            Powering data-driven decisions through innovative survey solutions
          </p>
          <Separator className="mb-8" />
          
          <Card className="mb-8">
            <CardContent className="pt-6">
              <h2 className="text-2xl font-semibold mb-4">Our Mission</h2>
              <p className="text-muted-foreground mb-6">
                At Execudata, we're committed to transforming how businesses collect, analyze, and act on customer 
                feedback. Our mission is to make data-driven decision making accessible to organizations of all sizes
                through intuitive survey tools and powerful analytics.
              </p>
              
              <h2 className="text-2xl font-semibold mb-4">Who We Are</h2>
              <p className="text-muted-foreground mb-6">
                Founded in 2018, Execudata has grown from a small startup to a leader in the customer feedback industry.
                Our team of data scientists, UX specialists, and customer experience experts work together to create
                solutions that bridge the gap between businesses and their customers.
              </p>
              
              <div className="grid md:grid-cols-3 gap-6 my-8">
                <div className="text-center p-4">
                  <div className="font-bold text-4xl text-primary mb-2">500+</div>
                  <p className="text-sm text-muted-foreground">Enterprise Clients</p>
                </div>
                <div className="text-center p-4">
                  <div className="font-bold text-4xl text-primary mb-2">10M+</div>
                  <p className="text-sm text-muted-foreground">Survey Responses</p>
                </div>
                <div className="text-center p-4">
                  <div className="font-bold text-4xl text-primary mb-2">32</div>
                  <p className="text-sm text-muted-foreground">Countries Served</p>
                </div>
              </div>
              
              <h2 className="text-2xl font-semibold mb-4">Our Approach</h2>
              <p className="text-muted-foreground mb-2">
                We believe that the best business decisions are informed by high-quality customer data. Our platform is designed with three core principles:
              </p>
              <ul className="list-disc pl-6 mb-6 text-muted-foreground space-y-2">
                <li><span className="font-medium text-foreground">Simplicity</span> — intuitive design that requires no technical expertise</li>
                <li><span className="font-medium text-foreground">Intelligence</span> — advanced analytics that surface meaningful insights</li>
                <li><span className="font-medium text-foreground">Action</span> — practical recommendations that drive business growth</li>
              </ul>
              
              <h2 className="text-2xl font-semibold mb-4">Our Technology</h2>
              <p className="text-muted-foreground mb-6">
                The SurveyMaster platform leverages cutting-edge technologies including machine learning for sentiment analysis,
                natural language processing to identify trends in open-ended responses, and predictive analytics to forecast
                customer behavior. Our secure cloud infrastructure ensures your data is always protected while remaining
                accessible when you need it.
              </p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="pt-6">
              <h2 className="text-2xl font-semibold mb-4">Contact Us</h2>
              <p className="text-muted-foreground mb-6">
                Have questions about Execudata or the SurveyMaster platform? We'd love to hear from you.
              </p>
              
              <div className="grid md:grid-cols-2 gap-6">
                <div>
                  <h3 className="font-medium mb-2">Headquarters</h3>
                  <address className="not-italic text-muted-foreground">
                    1234 Innovation Drive<br />
                    Suite 500<br />
                    San Francisco, CA 94107<br />
                    United States
                  </address>
                </div>
                <div>
                  <h3 className="font-medium mb-2">Get in Touch</h3>
                  <p className="text-muted-foreground mb-1">
                    Email: <a href="mailto:info@execudata.com" className="text-primary hover:underline">info@execudata.com</a>
                  </p>
                  <p className="text-muted-foreground">
                    Phone: <a href="tel:+15551234567" className="text-primary hover:underline">+1 (555) 123-4567</a>
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
