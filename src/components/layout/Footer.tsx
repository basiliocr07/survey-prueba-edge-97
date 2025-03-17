
import { Link } from 'react-router-dom';
import { Facebook, Linkedin, Twitter } from 'lucide-react';

export default function Footer() {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="w-full py-8 px-6 mt-auto border-t">
      <div className="max-w-7xl mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          <div className="space-y-4">
            <h4 className="text-lg font-semibold tracking-tight">SurveyMaster</h4>
            <p className="text-sm text-muted-foreground">
              Create beautiful surveys and analyze results with our simple yet powerful platform.
            </p>
            {/* Social Media Icons */}
            <div className="flex space-x-4 pt-2">
              <a 
                href="https://facebook.com" 
                target="_blank" 
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="Facebook"
              >
                <Facebook className="h-5 w-5" />
              </a>
              <a 
                href="https://linkedin.com" 
                target="_blank" 
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="LinkedIn"
              >
                <Linkedin className="h-5 w-5" />
              </a>
              <a 
                href="https://twitter.com" 
                target="_blank" 
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary transition-colors"
                aria-label="Twitter"
              >
                <Twitter className="h-5 w-5" />
              </a>
            </div>
          </div>
          
          <div>
            <h5 className="text-sm font-medium mb-4 text-muted-foreground">Product</h5>
            <ul className="space-y-2">
              <li>
                <Link to="/create" className="text-sm hover:text-primary transition-colors">
                  Create Surveys
                </Link>
              </li>
              <li>
                <Link to="/results" className="text-sm hover:text-primary transition-colors">
                  Analysis
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Templates
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Pricing
                </Link>
              </li>
            </ul>
          </div>
          
          <div>
            <h5 className="text-sm font-medium mb-4 text-muted-foreground">Resources</h5>
            <ul className="space-y-2">
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Documentation
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Guides
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  API
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Help Center
                </Link>
              </li>
            </ul>
          </div>
          
          <div>
            <h5 className="text-sm font-medium mb-4 text-muted-foreground">Company</h5>
            <ul className="space-y-2">
              <li>
                <Link to="/about" className="text-sm hover:text-primary transition-colors">
                  About
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Blog
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Careers
                </Link>
              </li>
              <li>
                <Link to="/" className="text-sm hover:text-primary transition-colors">
                  Contact
                </Link>
              </li>
            </ul>
          </div>
        </div>
        
        <div className="flex flex-col md:flex-row justify-between items-center mt-10 pt-6 border-t text-sm text-muted-foreground">
          <p>© {currentYear} SurveyMaster. All rights reserved.</p>
          <div className="flex space-x-6 mt-4 md:mt-0">
            <Link to="/" className="hover:text-primary transition-colors">Terms</Link>
            <Link to="/" className="hover:text-primary transition-colors">Privacy</Link>
            <Link to="/" className="hover:text-primary transition-colors">Cookies</Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
