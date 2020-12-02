//#include "GL/soil.h"
#define _USE_MATH_DEFINES
#include "GL/glut.h"
#include "GL/freeglut.h"
#include <iostream>
#include <math.h>
#include <limits.h>
#include "soil.h"
static int w = 0, h = 0;

GLfloat distX = 0, distY = 0;
GLfloat angle = 0;

GLfloat cam_dist = 20;
GLfloat ang_hor = 0, ang_vert = -60;

double camX = 0;
double camY = 0;
double camZ = 0;

double carX = 5;
double carY = 5;
double carAngle = 0;


float amb[] = { 0.8, 0.8, 0.8 };
float dif[] = { 0.2, 0.2, 0.2 };

GLfloat no_light[] = { 0, 0, 0, 1 };
GLfloat light[] = { 1, 1, 1, 0 };

unsigned int texture = 0;
unsigned int sphere_texture = 0;

void loadTextures() {
	texture = SOIL_load_OGL_texture("2.jpg", SOIL_LOAD_AUTO, SOIL_CREATE_NEW_ID,
		SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB | SOIL_FLAG_COMPRESS_TO_DXT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

	sphere_texture = SOIL_load_OGL_texture("3.jpg", SOIL_LOAD_AUTO, SOIL_CREATE_NEW_ID,
		SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB | SOIL_FLAG_COMPRESS_TO_DXT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
}

void init() {
    glClearColor(0, 0, 0, 1);
    glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE | GLUT_DEPTH);

	const GLfloat light_diffuse[] = { 1.0, 1.0, 1.0, 1.0 };
	const GLfloat light_specular[] = { 1.0, 1.0, 1.0, 1.0 };
	glLightfv(GL_LIGHT1, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT1, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT2, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT2, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT3, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT3, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT4, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT5, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT5, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT6, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT6, GL_DIFFUSE, light_diffuse);
	glLightfv(GL_LIGHT7, GL_SPECULAR, light_specular);
	glLightfv(GL_LIGHT7, GL_DIFFUSE, light_diffuse);

    glEnable(GL_DEPTH_TEST);
    glEnable(GL_COLOR_MATERIAL);
	glEnable(GL_LIGHTING);
	glEnable(GL_LIGHT0);
	loadTextures();
}


void drawFloor() 
{
	glEnable(GL_TEXTURE_2D);
	glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);

	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);

	glBindTexture(GL_TEXTURE_2D, texture);

    glBegin(GL_QUADS);
    glTexCoord2f(0, 0); glNormal3f(0, 0, 1); glVertex3f(-10, -10, 0);
    glTexCoord2f(0, 1); glNormal3f(0, 0, 1); glVertex3f(-10, 10, 0);
    glTexCoord2f(1, 1); glNormal3f(0, 0, 1); glVertex3f(10, 10, 0);
    glTexCoord2f(1, 0); glNormal3f(0, 0, 1); glVertex3f(10, -10, 0);
    glEnd();

	glDisable(GL_TEXTURE_2D);
}


void drawSphere()
{
	GLfloat lamp_light[] = { 0.2f, 0.2f, 0.2f, 0 };
	glEnable(GL_TEXTURE_2D);
	glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);

	//glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	//glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);

	glBindTexture(GL_TEXTURE_2D, sphere_texture);
	glMaterialf(GL_FRONT, GL_SHININESS, 10);

	glMaterialfv(GL_FRONT, GL_SPECULAR, lamp_light);
	glutSolidSphere(0.3, 8, 8);
	glDisable(GL_TEXTURE_2D);
}

void drawLamp(GLfloat x, GLfloat y) 
{
	GLfloat lamp_light[] = { 0.2f, 0.2f, 0.2f, 0 };
	glColor3f(0.5f, 0.5f, 0.5f);
    glPushMatrix();
    glTranslatef(x, y, 0);
	if (glIsEnabled(GL_LIGHT1))
		glMaterialfv(GL_FRONT, GL_DIFFUSE, lamp_light);
	else
		glMaterialfv(GL_FRONT, GL_DIFFUSE, no_light);
    glutSolidCylinder(0.1, 4, 8, 8);
    glPushMatrix();
    glTranslatef(0, 0, 4.1);
	if (glIsEnabled(GL_LIGHT1))
		glMaterialfv(GL_FRONT, GL_EMISSION, lamp_light);
	else
		glMaterialfv(GL_FRONT, GL_EMISSION, no_light);
    glutSolidSphere(0.6, 8, 8);
    glPopMatrix();
    glPopMatrix();
}

void drawCar(GLfloat x = 5, GLfloat y = 5)
{
	const GLfloat light_pos[] = { 1.f, 0.f, 0.f };
	GLfloat dir[] = { 1, 0, 0, 1 };
	GLfloat pos[] = { 0,0,0 };

	glPushMatrix();
	glTranslated(x, y, 0);
	glRotated(carAngle, 0, 0, 1);

	glPushMatrix();
	glTranslated(distX, distY, 1);
	glRotated(angle, 0, 0, 1);
	//Кузов
	glColor3f(1.0, 0.0, 0.0);
	glPushMatrix();
	glScaled(2, 1, 1);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidCube(1);
	glPopMatrix();
	glColor3f(1.0, 1.0, 1.0);
	//Задние колеса
	glColor3f(0.0, 0.0, 0.0);
	glPushMatrix();
	glTranslated(-0.5, -0.5, -0.6);
	glRotated(90, 1, 0, 0);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glTranslated(0, 0, -1);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glPopMatrix();
	glColor3f(1.0, 1.0, 1.0);
	//Кабина
	glColor3f(0.0, 0.0, 1.0);
	glPushMatrix();
	glTranslated(1.3, 0, -0.1);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidCube(0.6);
	glColor3f(1.0, 1.0, 1.0);
	//Фары
	glColor3f(0.6, 0.6, 0.6);
	glPushMatrix();
	glTranslated(0.4, 0.3, 0);
	if (glIsEnabled(GL_LIGHT5))
		glMaterialfv(GL_FRONT, GL_EMISSION, light);
	else
		glMaterialfv(GL_FRONT, GL_EMISSION, no_light);
	glutSolidSphere(0.05,20,20);
	glTranslated(0, -0.6, 0);
	glutSolidSphere(0.05,20,20);
	glLightfv(GL_LIGHT5, GL_POSITION, pos);
	glLightf(GL_LIGHT5, GL_SPOT_CUTOFF, 60);
	glLightfv(GL_LIGHT5, GL_SPOT_DIRECTION, dir);
	glMaterialfv(GL_FRONT, GL_EMISSION, no_light);
	glPopMatrix();
	glColor3f(1.0, 1.0, 1.0);
	//Передние колеса
	glColor3f(0.0, 0.0, 0.0);
	glTranslated(-0.6, -0.5, -0.5);
	glRotated(90, 1, 0, 0);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidTorus(0.15,0.2,20,20);
	glTranslated(0, 0,-1);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, amb);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, dif);
	glutSolidTorus(0.15, 0.2, 20, 20);
	glColor3f(1.0, 1.0, 1.0);
	glPopMatrix();

	glPopMatrix();
	glPopMatrix();
    glPopMatrix();
}

void drawCristmasTree()
{
	GLfloat sphere_light[] = { 0.0005f, 0.0005f, 0.0005f, 0 };
	const GLfloat light_pos[] = { 0.f, 0.f, 4.6f, 1.f };
    //ствол
	glColor3f(0.5f, 0.2f, 0.0f);
    glutSolidCylinder(0.8, 5, 6, 2);
	glColor3f(1.0f, 1.0f, 1.0f);
    
    //конусы(иголочки)
    
	glColor3f(0.0f, 0.5f, 0.0f);
	glPushMatrix();
	glTranslated(0, 0, 1.5);
	GLUquadricObj* bot = gluNewQuadric();
	gluQuadricDrawStyle(bot, GLU_FILL);
	gluCylinder(bot, 3, 0.1, 4, 16, 16);
    glPopMatrix();

	glColor3f(0.0f, 0.8f, 0.0f);
	glPushMatrix();
	glTranslated(0, 0, 3);
	GLUquadricObj* middle = gluNewQuadric();
	gluQuadricDrawStyle(middle, GLU_FILL);
	gluCylinder(middle, 2, 0.1, 4, 16, 16);
	glPopMatrix();

	glColor3f(0.0f, 1.0f, 0.0f);
	glPushMatrix();
	glTranslated(0, 0, 4.2);
	GLUquadricObj* top = gluNewQuadric();
	gluQuadricDrawStyle(top, GLU_FILL);
	gluCylinder(top, 1.5, 0.1, 4, 16, 16);
	glPopMatrix();
	
	glColor3f(1.0f, 1.0f, 1.0f);
	
    //шарики
	glPushMatrix();
	glTranslated(1.65, 1, 3.2);
    //glutSolidSphere(0.3, 8, 8);
	drawSphere();

	glTranslated(-3, -0.77, 2);
	//glutSolidSphere(0.3, 8, 8);
	drawSphere();

	glTranslated(1.8, -1.8, -1);
	//glutSolidSphere(0.3, 8, 8);
	drawSphere();

    glPopMatrix();

    //герлянды
	glPushMatrix();
    double startX = 2.57;
    double startY = 1.12;
	glTranslated(-0.5, -2.5, 2.2);
    for (int i = 0; i < 34 - 16; i++)
    {
        glTranslated(0.7, 0.0, 0);
	    glRotated(360/24, 0, 0, 1);
	    glRotated(360/80, 1, 0, 0);
	    glColor3f((rand() % 255) / 255.0, (rand() % 255) / 255.0, (rand() % 255) / 255.0);
		if (glIsEnabled(GL_LIGHT4))
			glMaterialfv(GL_FRONT, GL_EMISSION, sphere_light);
		else
			glMaterialfv(GL_FRONT, GL_EMISSION, no_light);
        glutSolidSphere(0.1, 6, 6);
	    glColor3f(1.0, 1.0, 1.0);
    }
    glPopMatrix();
}

void update() {
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    glLoadIdentity();

    double ang_vert_r = ang_vert / 180 * M_PI;
    double ang_hor_r = ang_hor / 180 * M_PI;
    camX = cam_dist * std::sin(ang_vert_r) * std::cos(ang_hor_r);
    camY = cam_dist * std::sin(ang_vert_r) * std::sin(ang_hor_r);
    camZ = cam_dist * std::cos(ang_vert_r);

    gluLookAt(camX, camY, camZ, 0., 0., 0., 0., 0., 1.);

    drawLamp(7, -7);
	drawLamp(7, 7);
	drawLamp(-7, -7);
	drawLamp(-7, 7);


    drawFloor();

    drawCristmasTree();

	drawCar(carX, carY);

    glFlush();
    glutSwapBuffers();
    glutPostRedisplay();
}

void updateCamera() {
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(60.f, (float)w / h, 1.0f, 1000.f);
    glMatrixMode(GL_MODELVIEW);
}


void keyboard(unsigned char key, int x, int y) {
    switch (key) {
    case 'w':
        ang_vert += 5;
        break;
    case 's':
        ang_vert -= 5;
        break;
    case 'a':
        ang_hor -= 5;
        break;
    case 'd':
        ang_hor += 5;
        break;
	case '1'://фонари
		if (glIsEnabled(GL_LIGHT1))
			glDisable(GL_LIGHT1);
		else
			glEnable(GL_LIGHT1);
		break;
	case '2'://гирлянды
		if (glIsEnabled(GL_LIGHT4))
			glDisable(GL_LIGHT4);
		else
			glEnable(GL_LIGHT4);
		break;
	case '3'://фары
		if (glIsEnabled(GL_LIGHT5))
			glDisable(GL_LIGHT5);
		else
			glEnable(GL_LIGHT5);
		break;
	case '4'://верхний глобальный свет(лучше вырубить, чтоб остальное лучше видеть)
		if (glIsEnabled(GL_LIGHT0))
			glDisable(GL_LIGHT0);
		else
			glEnable(GL_LIGHT0);
		break;
	case 'l':
		carAngle -= 10;
		break;
	case 'j':
		carAngle += 10;
		break;
	case 'i':
		carX += std::cos(carAngle / 180 * M_PI) * 0.3;
		carY += std::sin(carAngle / 180 * M_PI) * 0.3;
		break;
	case 'k':
		carX -= std::cos(carAngle / 180 * M_PI) * 0.3;
		carY -= std::sin(carAngle / 180 * M_PI) * 0.3;
		break;
	default:
		break;
    }
    glutPostRedisplay();
}

void specialKeys(int key, int x, int y) {
	/////*switch (key)
	////{
	////case GLUT_KEY_RIGHT:
	////	carAngle -= 10;
	////	break;
	////case GLUT_KEY_LEFT:
	////	carAngle += 10;
	////	break;
	////case GLUT_KEY_UP:
	////	carX += std::cos(carAngle/180 * M_PI) * 0.3;
	////	carY += std::sin(carAngle/180 * M_PI) * 0.3;
	////	break;
	////case GLUT_KEY_DOWN:
	////	carX -= std::cos(carAngle/180 * M_PI) * 0.3;
	////	carY -= std::sin(carAngle/180 * M_PI) * 0.3;
	////	break;
	////default:
	////	break;
	////}
	////std::cout << carX << "   " << carY << std::endl;*/
}

void reshape(int width, int height) {
    w = width;
    h = height;

    glViewport(0, 0, w, h);
    updateCamera();
}


int main(int argc, char* argv[]) {
    glutInit(&argc, argv);
    glutInitWindowPosition(100, 100);
    glutInitWindowSize(800, 800);
    glutCreateWindow("texture and lighting");

    init();

    glutReshapeFunc(reshape);
    glutDisplayFunc(update);
    glutKeyboardFunc(keyboard);
	glutSpecialFunc(specialKeys);
    glutMainLoop();

    return 0;
}
